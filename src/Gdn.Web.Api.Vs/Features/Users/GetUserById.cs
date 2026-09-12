using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Users;

public class GetUserById
{
    public record GetUserByIdResponse(int Id, string Email, string? FullName, string Role, bool IsActive);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/users/{id:int}", HandlerAsync).WithTags(Tags.Users).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(int id, IUserRepository userRepository)
    {
        var data = await userRepository.GetAsync(id);

        return data is not null
            ? ResultHelper.Ok(MapResponse(data))
            : ResultHelper.NotFound(UserErrors.NotFound(id));
    }

    private static GetUserByIdResponse MapResponse(User entity)
        => new(entity.Id, entity.Email, entity.FullName, entity.Role, entity.IsActive);
}
