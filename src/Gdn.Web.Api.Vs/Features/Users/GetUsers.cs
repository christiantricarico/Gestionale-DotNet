using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Users;

public class GetUsers
{
    public record GetUsersResponse(
        int Id,
        string Email,
        string? FullName,
        string Role,
        bool IsActive,
        bool IsLockedOut,
        DateTime? LastLoginAt);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/users", HandlerAsync).WithTags(Tags.Users).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(IUserRepository userRepository)
    {
        var data = await userRepository.GetAllAsync();

        return ResultHelper.Ok(data.Select(MapResponse));
    }

    private static GetUsersResponse MapResponse(User entity)
        => new(entity.Id, entity.Email, entity.FullName, entity.Role, entity.IsActive, entity.IsLockedOut, entity.LastLoginAt);
}
