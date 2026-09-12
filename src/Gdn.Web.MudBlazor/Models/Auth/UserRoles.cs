namespace Gdn.Web.MudBlazor.Models.Auth;

/// <summary>
/// Role names as issued by the API. Hiding menu entries by role is a convenience only: the
/// WebAssembly code is downloadable, so the real check is always the one on the server.
/// </summary>
public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Coworker = "Coworker";
}
