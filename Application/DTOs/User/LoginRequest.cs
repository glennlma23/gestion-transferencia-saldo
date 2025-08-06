using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User;

public class LoginRequest
{
    /// <summary>
    /// Nombre de usuario registrado.
    /// </summary>
    /// <example>glennlma23</example>
    [RegularExpression("^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ ]+$", ErrorMessage = "Name solo debe contener letras, números y espacios.")]
    [Required(ErrorMessage = "Username no puede ser vacío.")]
    public string Username { get; set; } = string.Empty;
    /// <summary>
    /// Contraseña de usuario registrado.
    /// </summary>
    /// <example>j3jkt9jn0nnfb1</example>
    [Required(ErrorMessage = "Password no puede ser vacío.")]
    public string Password { get; set; } = string.Empty;
}