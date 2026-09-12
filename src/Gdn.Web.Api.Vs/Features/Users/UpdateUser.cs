using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Users;

public class UpdateUser
{
    public record UpdateUserRequest(int Id, string Email, string? FullName, string Role, bool IsActive);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/users", HandlerAsync).WithTags(Tags.Users).RequireAuthorization(Policies.AdminOnly);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateUserRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).GreaterThan(0);
            RuleFor(e => e.Email).NotEmpty().MaximumLength(255).EmailAddress();
            RuleFor(e => e.FullName).MaximumLength(255);
            RuleFor(e => e.Role).NotEmpty()
                .Must(role => role == UserRole.Admin || role == UserRole.Coworker)
                .WithMessage($"Role must be '{UserRole.Admin}' or '{UserRole.Coworker}'");
        }
    }

    private static async Task<IResult> HandlerAsync(
        UpdateUserRequest request,
        IValidator<UpdateUserRequest> validator,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var user = await userRepository.GetAsync(request.Id);
        if (user is null)
            return ResultHelper.NotFound(UserErrors.NotFound(request.Id));

        var email = request.Email.Trim().ToLowerInvariant();

        if (email != user.Email)
        {
            var existing = await userRepository.GetByEmailAsync(email);
            if (existing is not null)
                return ResultHelper.Conflict(UserErrors.EmailAlreadyExists(email));
        }

        var losesAdminAccess = user.IsActive && user.Role == UserRole.Admin
            && (request.Role != UserRole.Admin || !request.IsActive);

        if (losesAdminAccess && await userRepository.CountActiveAdminsExcludingAsync(user.Id) == 0)
            return ResultHelper.Conflict(UserErrors.LastAdmin());

        // A changed role or a deactivation must take effect now, not whenever the current access
        // token happens to expire, so every session of this user is ended.
        var mustEndSessions = request.Role != user.Role || (!request.IsActive && user.IsActive);
        var isReactivation = request.IsActive && !user.IsActive;

        user.Email = email;
        user.FullName = request.FullName;
        user.Role = request.Role;
        user.IsActive = request.IsActive;

        if (isReactivation)
        {
            user.FailedAccessAttempts = 0;
            user.LockoutEndAt = null;
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync();

        // Done after saving: revoking runs as its own statement, so it must not happen while the
        // change to the user could still fail.
        if (mustEndSessions)
            await refreshTokenRepository.RevokeAllForUserAsync(user.Id);

        return ResultHelper.NoContent();
    }
}
