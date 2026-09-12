using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Users;

public class UserViewModel
{
    public int Id { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; } = default!;

    [Display(Name = "Nome")]
    public string? FullName { get; set; }

    [Display(Name = "Ruolo")]
    public string Role { get; set; } = default!;

    [Display(Name = "Attivo")]
    public bool IsActive { get; set; }

    [Display(Name = "Bloccato")]
    public bool IsLockedOut { get; set; }

    [Display(Name = "Ultimo accesso")]
    public DateTime? LastLoginAt { get; set; }
}

public class UserEditModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Email richiesta.")]
    [EmailAddress(ErrorMessage = "Email non valida.")]
    [StringLength(255)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Nome")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Ruolo richiesto.")]
    [Display(Name = "Ruolo")]
    public string Role { get; set; } = UserRoles.Coworker;

    [Display(Name = "Attivo")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Only used when creating a user. An existing password is never loaded and never shown: it is
    /// replaced through the dedicated reset instead.
    /// </summary>
    [Display(Name = "Password")]
    public string? Password { get; set; }
}

public class ResetPasswordModel
{
    public int Id { get; set; }

    [Display(Name = "Utente")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password richiesta.")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "La password deve avere almeno 8 caratteri.")]
    [Display(Name = "Nuova password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Conferma la password.")]
    [Compare(nameof(Password), ErrorMessage = "Le password non coincidono.")]
    [Display(Name = "Conferma password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
