using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Wallet;

public class CreateWalletRequest
{
    /// <summary>
    /// Documento de identidad de la persona propietaria de la billetera.
    /// </summary>
    /// <example>72319090</example>
    [Required(ErrorMessage = "DocumentId no puede ser vac�o.")]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "DocumentId solo debe contener letras y n�meros.")]
    public string DocumentId { get; set; } = null!;
    /// <summary>
    /// Nombre del propietario de la billetera.
    /// </summary>
    /// <example>Glenn Maura</example>
    [Required(ErrorMessage = "Name no puede ser vac�o.")]
    [RegularExpression("^[a-zA-Z0-9������������ ]+$", ErrorMessage = "Name solo debe contener letras, n�meros y espacios.")]
    public string Name { get; set; } = null!;
}