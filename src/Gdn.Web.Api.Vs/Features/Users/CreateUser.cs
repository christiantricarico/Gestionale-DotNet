using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.AspNetCore.Identity;

namespace Gdn.Web.Api.Vs.Features.Users;

public class CreateUser
{
    public record CreateUserRequest(string Email, string Password, string? FullName, string Role, bool IsActive);

    public record CreateUserResponse(int Id, string Email, string? FullName, string Role, bool IsActive);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/users", HandlerAsync).WithTags(Tags.Users).RequireAuthorization(Policies.AdminOnly);
        }
    }

    public sealed class Validator : AbstractValidator<CreateUserRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Email).NotEmpty().MaximumLength(255).EmailAddress();
            RuleFor(e => e.Password).NotEmpty().MinimumLength(8).MaximumLength(255);
            RuleFor(e => e.FullName).MaximumLength(255);
            RuleFor(e => e.Role).NotEmpty()
                .Must(role => role == UserRole.Admin || role == UserRole.Coworker)
                .WithMessage($"Role must be '{UserRole.Admin}' or '{UserRole.Coworker}'");
        }
    }

    private static async Task<IResult> HandlerAsync(
        CreateUserRequest request,
        IValidator<CreateUserRequest> validator,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var email = request.Email.Trim().ToLowerInvariant();

        var existing = await userRepository.GetByEmailAsync(email);
        if (existing is not null)
            return ResultHelper.Conflict(UserErrors.EmailAlreadyExists(email));

        var user = new User
        {
            Email = email,
            FullName = request.FullName,
            Role = request.Role,
            IsActive = request.IsActive
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        unitOfWork.GetRepository<IUserRepository>().Add(user);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Created(new CreateUserResponse(user.Id, user.Email, user.FullName, user.Role, user.IsActive));
    }
}
