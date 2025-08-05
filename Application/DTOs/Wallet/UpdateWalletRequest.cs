using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Wallet;

public class UpdateWalletRequest
{
    /// <summary>
    /// Nombre del propietario de la billetera.
    /// </summary>
    /// <example>Glenn Maura</example>
    [Required(ErrorMessage = "Name no puede ser vacío.")]
    public string Name { get; set; } = null!;
}