namespace Gdn.Domain.Models.Enums;

/// <summary>
/// Application roles. Unlike the other domain constants these carry readable values, because they
/// are written into the <c>role</c> claim of the access token and show up in every request.
/// </summary>
public static class UserRole
{
    public const string Admin = "Admin";
    public const string Coworker = "Coworker";
}
