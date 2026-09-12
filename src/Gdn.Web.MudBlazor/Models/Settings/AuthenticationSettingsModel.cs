using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Settings;

public class AuthenticationSettingsModel
{
    [Required(ErrorMessage = "Valore richiesto.")]
    [Range(1, 120, ErrorMessage = "Indicare un valore tra 1 e 120 minuti.")]
    [Display(Name = "Durata del token di accesso (minuti)")]
    public int AccessTokenMinutes { get; set; } = 15;

    [Required(ErrorMessage = "Valore richiesto.")]
    [Range(1, 43200, ErrorMessage = "Indicare un valore tra 1 e 43200 minuti.")]
    [Display(Name = "Timeout di inattività (minuti)")]
    public int SessionIdleTimeoutMinutes { get; set; } = 480;

    [Required(ErrorMessage = "Valore richiesto.")]
    [Range(1, 50, ErrorMessage = "Indicare un valore tra 1 e 50.")]
    [Display(Name = "Tentativi falliti prima del blocco")]
    public int MaxFailedAccessAttempts { get; set; } = 5;

    [Required(ErrorMessage = "Valore richiesto.")]
    [Range(1, 1440, ErrorMessage = "Indicare un valore tra 1 e 1440 minuti.")]
    [Display(Name = "Durata del blocco (minuti)")]
    public int LockoutMinutes { get; set; } = 15;
}
