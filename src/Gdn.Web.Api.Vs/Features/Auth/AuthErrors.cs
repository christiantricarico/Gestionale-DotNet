namespace Gdn.Web.Api.Vs.Features.Auth;

public static class AuthErrors
{
    /// <summary>
    /// Deliberately the same message for wrong password, unknown email and deactivated account, so
    /// the endpoint cannot be used to find out who is registered.
    /// </summary>
    public static Error InvalidCredentials() => new("Auth:InvalidCredentials", "Email o password non corretti.");

    /// <summary>
    /// Reports the remaining time rather than an absolute hour, which would be the time zone of the
    /// server and not necessarily the one the person is reading it in.
    /// </summary>
    public static Error AccountLocked(TimeSpan remaining)
    {
        var minutes = Math.Max(1, (int)Math.Ceiling(remaining.TotalMinutes));

        return new("Auth:AccountLocked",
            $"Account bloccato per troppi tentativi falliti. Riprova fra {minutes} {(minutes == 1 ? "minuto" : "minuti")}.");
    }

    public static Error InvalidRefreshToken() => new("Auth:InvalidRefreshToken", "Sessione non valida o scaduta. Esegui nuovamente l'accesso.");
}
