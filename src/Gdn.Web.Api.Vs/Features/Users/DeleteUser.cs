using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Users;

public class DeleteUser
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/users/{id:int}", HandlerAsync).WithTags(Tags.Users).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        var user = await userRepository.GetAsync(id);
        if (user is null)
            return ResultHelper.NotFound(UserErrors.NotFound(id));

        if (user.IsActive && user.Role == UserRole.Admin
            && await userRepository.CountActiveAdminsExcludingAsync(user.Id) == 0)
        {
            return ResultHelper.Conflict(UserErrors.LastAdmin());
        }

        // The refresh tokens of this user go with it, through the cascade on the relationship.
        await unitOfWork.GetRepository<IUserRepository>().RemoveAsync(id);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.NoContent();
    }
}
