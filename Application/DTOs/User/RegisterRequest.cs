using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User;

public class RegisterRequest
{
    /// <summary>
    /// Contraseña de usuario registrado.
    /// </summary>
    /// <example>Glenn Maura</example>
    [Required(ErrorMessage = "Username no puede ser vacío.")]
    public string Username { get; set; } = default!;
    /// <summary>
    /// Contraseña de usuario registrado.
    /// </summary>
    /// <example>j3jkt9jn0nnfb1</example>
    [Required(ErrorMessage = "Password no puede ser vacío.")]
    public string Password { get; set; } = default!;
    /// <summary>
    /// Documento de identidad de usuario a registrarse.
    /// </summary>
    /// <example>72319090</example>
    [Required(ErrorMessage = "DocumentId no puede ser vacío.")]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "DocumentId solo debe contener letras y números.")]
    public string DocumentId { get; set; } = default!;
}