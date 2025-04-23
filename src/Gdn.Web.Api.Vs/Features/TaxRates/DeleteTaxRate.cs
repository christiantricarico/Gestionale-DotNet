using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class DeleteTaxRate
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/taxrates/{id:int}", Handler).WithTags(Tags.TaxRates);
        }
    }

    private static async Task<IResult> Handler(int id, IUnitOfWork unitOfWork)
    {
        var taxRateRepository = unitOfWork.GetRepository<ITaxRateRepository>();
        await taxRateRepository.RemoveAsync(id);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
