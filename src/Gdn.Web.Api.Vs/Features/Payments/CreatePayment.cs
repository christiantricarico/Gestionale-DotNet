using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Payments;

public class CreatePayment
{
    public record AllocationRequest(int DueId, decimal Amount);
    public record Request(DateOnly Date, decimal Amount, int? PaymentMethodId, int? CustomerId, IEnumerable<AllocationRequest> Allocations);

    public record AllocationResponse(int DueId, decimal Amount);
    public record Response(int Id, DateOnly Date, decimal Amount, int? PaymentMethodId, int? CustomerId, IEnumerable<AllocationResponse> Allocations);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/payments", Handler).WithTags(Tags.Payments);
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(e => e.Allocations).NotEmpty();
            RuleForEach(e => e.Allocations).ChildRules(a =>
            {
                a.RuleFor(x => x.Amount).NotEqual(0);
            });
        }
    }

    private static async Task<IResult> Handler(Request request, IValidator<Request> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var allocationSum = request.Allocations.Sum(a => a.Amount);

        // When amount is zero it must be a compensation: individual allocations can be non-zero
        // but must cancel out. For non-zero amounts, sign and cap are enforced.
        if (request.Amount == 0m)
        {
            if (allocationSum != 0m)
                return ResultHelper.BadRequest(PaymentErrors.AllocationExceedsPayment());
        }
        else
        {
            if (Math.Sign(allocationSum) != Math.Sign(request.Amount))
                return ResultHelper.BadRequest(PaymentErrors.AllocationExceedsPayment());

            if ((request.Amount > 0m && allocationSum > request.Amount)
                || (request.Amount < 0m && allocationSum < request.Amount))
                return ResultHelper.BadRequest(PaymentErrors.AllocationExceedsPayment());
        }

        if (request.CustomerId.HasValue)
        {
            var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();
            var customer = await customerRepository.GetAsync(request.CustomerId.Value);
            if (customer is null)
                return ResultHelper.NotFound(PaymentErrors.CustomerNotFound(request.CustomerId.Value));
        }

        var dueRepository = unitOfWork.GetRepository<IDueRepository>();

        var paymentDues = new List<PaymentDue>();

        foreach (var allocation in request.Allocations)
        {
            var due = await dueRepository.GetAsync(allocation.DueId);
            if (due is null)
                return ResultHelper.BadRequest(PaymentErrors.DueNotFound(allocation.DueId));

            if (request.CustomerId.HasValue && due.CustomerId != request.CustomerId)
                return ResultHelper.BadRequest(PaymentErrors.DueNotBelongToCustomer(allocation.DueId, request.CustomerId.Value));

            if (Math.Sign(allocation.Amount) != Math.Sign(due.Amount))
                return ResultHelper.BadRequest(PaymentErrors.AllocationSignMismatch(allocation.DueId));

            var newPaidAmount = due.PaidAmount + allocation.Amount;
            if ((due.Amount > 0m && newPaidAmount > due.Amount)
                || (due.Amount < 0m && newPaidAmount < due.Amount))
                return ResultHelper.BadRequest(PaymentErrors.AllocationExceedsDue(allocation.DueId));

            paymentDues.Add(new PaymentDue { DueId = allocation.DueId, Amount = allocation.Amount });
        }

        var payment = new Payment
        {
            Date = request.Date,
            Amount = request.Amount,
            PaymentMethodId = request.PaymentMethodId,
            CustomerId = request.CustomerId,
            PaymentDues = paymentDues
        };

        var paymentRepository = unitOfWork.GetRepository<IPaymentRepository>();
        paymentRepository.Add(payment);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(MapResponse(payment));
    }

    private static Response MapResponse(Payment p)
        => new(p.Id, p.Date, p.Amount, p.PaymentMethodId, p.CustomerId,
               p.PaymentDues.Select(pd => new AllocationResponse(pd.DueId, pd.Amount)));
}
