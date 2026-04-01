using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Payments;

public class CreatePayment
{
    public record AllocationRequest(int DueId, decimal Amount);
    public record Request(DateOnly Date, decimal Amount, int? PaymentMethodId, IEnumerable<AllocationRequest> Allocations);

    public record AllocationResponse(int DueId, decimal Amount);
    public record Response(int Id, DateOnly Date, decimal Amount, int? PaymentMethodId, IEnumerable<AllocationResponse> Allocations);

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
            RuleFor(e => e.Amount).GreaterThan(0);
            RuleFor(e => e.Allocations).NotEmpty();
            RuleForEach(e => e.Allocations).ChildRules(a =>
            {
                a.RuleFor(x => x.Amount).GreaterThan(0);
            });
        }
    }

    private static async Task<IResult> Handler(Request request, IValidator<Request> validator, IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var allocationSum = request.Allocations.Sum(a => a.Amount);
        if (allocationSum > request.Amount)
            return ResultHelper.BadRequest(PaymentErrors.AllocationExceedsPayment());

        var dueRepository = unitOfWork.GetRepository<IDueRepository>();

        var paymentDues = new List<PaymentDue>();

        foreach (var allocation in request.Allocations)
        {
            var due = await dueRepository.GetAsync(allocation.DueId);
            if (due is null)
                return ResultHelper.BadRequest(PaymentErrors.DueNotFound(allocation.DueId));

            if (due.PaidAmount + allocation.Amount > due.Amount)
                return ResultHelper.BadRequest(PaymentErrors.AllocationExceedsDue(allocation.DueId));

            paymentDues.Add(new PaymentDue { DueId = allocation.DueId, Amount = allocation.Amount });
        }

        var payment = new Payment
        {
            Date = request.Date,
            Amount = request.Amount,
            PaymentMethodId = request.PaymentMethodId,
            PaymentDues = paymentDues
        };

        var paymentRepository = unitOfWork.GetRepository<IPaymentRepository>();
        paymentRepository.Add(payment);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(MapResponse(payment));
    }

    private static Response MapResponse(Payment p)
        => new(p.Id, p.Date, p.Amount, p.PaymentMethodId,
               p.PaymentDues.Select(pd => new AllocationResponse(pd.DueId, pd.Amount)));
}
