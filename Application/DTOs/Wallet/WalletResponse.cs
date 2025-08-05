namespace Application.DTOs.Wallet;

public class WalletResponse
{
    /// <summary>
    /// Identificador único.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }
    /// <summary>
    /// Documento de identidad de la persona propietaria de la billetera.
    /// </summary>
    /// <example>72319090</example>
    public string DocumentId { get; set; } = null!;
    /// <summary>
    /// Nombre del propietario de la billetera.
    /// </summary>
    /// <example>Glenn Maura</example>
    public string Name { get; set; } = null!;
    /// <summary>
    /// Saldo de la billetera.
    /// </summary>
    /// <example>0</example>
    public decimal Balance { get; set; }
    /// <summary>
    /// Fecha de apertura de la billetera.
    /// </summary>
    /// <example>05/08/2025 11:11:11</example>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Fecha de última actualización de la billetera.
    /// </summary>
    /// <example>05/08/2025 11:11:11</example>
    public DateTime UpdatedAt { get; set; }
}