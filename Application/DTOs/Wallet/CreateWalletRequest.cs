using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Wallet;

public class CreateWalletRequest
{
    /// <summary>
    /// Nombre del propietario de la billetera.
    /// </summary>
    /// <example>Glenn Maura</example>
    [Required(ErrorMessage = "Name no puede ser vac�o.")]
    [RegularExpression("^[a-zA-Z0-9������������ ]+$", ErrorMessage = "Name solo debe contener letras, n�meros y espacios.")]
    public string Name { get; set; } = null!;
}