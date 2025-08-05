using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Wallet;

public class CreateWalletRequest
{
    /// <summary>
    /// Documento de identidad de la persona propietaria de la billetera.
    /// </summary>
    /// <example>72319090</example>
    [Required(ErrorMessage = "DocumentId no puede ser vacío.")]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "DocumentId solo debe contener letras y números.")]
    public string DocumentId { get; set; } = null!;
    /// <summary>
    /// Nombre del propietario de la billetera.
    /// </summary>
    /// <example>Glenn Maura</example>
    [Required(ErrorMessage = "Name no puede ser vacío.")]
    [RegularExpression("^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ ]+$", ErrorMessage = "Name solo debe contener letras, números y espacios.")]
    public string Name { get; set; } = null!;
}