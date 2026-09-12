using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.AspNetCore.Identity;

namespace Gdn.Web.Api.Vs.Features.Users;

/// <summary>
/// Sets a new password for a user. This is the only way a password ever changes: coworkers cannot
/// change their own, so the whole flow lives behind the administrator policy.
/// </summary>
public class ResetUserPassword
{
    public record ResetUserPasswordRequest(int Id, string Password);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/users/password", HandlerAsync).WithTags(Tags.Users).RequireAuthorization(Policies.AdminOnly);
        }
    }

    public sealed class Validator : AbstractValidator<ResetUserPasswordRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).GreaterThan(0);
            RuleFor(e => e.Password).NotEmpty().MinimumLength(8).MaximumLength(255);
        }
    }

    private static async Task<IResult> HandlerAsync(
        ResetUserPasswordRequest request,
        IValidator<ResetUserPasswordRequest> validator,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var user = await userRepository.GetAsync(request.Id);
        if (user is null)
            return ResultHelper.NotFound(UserErrors.NotFound(request.Id));

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        // A password reset also clears a lockout, which is how an administrator unblocks someone.
        user.FailedAccessAttempts = 0;
        user.LockoutEndAt = null;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync();

        // Existing sessions must not survive a password change.
        await refreshTokenRepository.RevokeAllForUserAsync(user.Id);

        return ResultHelper.NoContent();
    }
}
