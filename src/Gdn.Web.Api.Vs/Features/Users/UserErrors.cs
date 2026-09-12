namespace Gdn.Web.Api.Vs.Features.Users;

public static class UserErrors
{
    public static Error NotFound(int id) => new("User:NotFound", $"Utente con Id={id} non trovato.");

    public static Error EmailAlreadyExists(string email) => new("User:EmailAlreadyExists", $"Esiste gia un utente con email {email}.");

    public static Error InvalidRole(string role) => new("User:InvalidRole", $"Ruolo {role} non valido.");

    /// <summary>
    /// Guards the only remaining administrator. Without it the system could be left with nobody able
    /// to manage users, and no way back in.
    /// </summary>
    public static Error LastAdmin() => new("User:LastAdmin", "Operazione non consentita: deve restare almeno un amministratore attivo.");
}
