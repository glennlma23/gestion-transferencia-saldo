namespace Application.DTOs.Movement
{
    public class MovementResponse
    {
        /// <summary>
        /// Identificador �nico del movimiento.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }
        /// <summary>
        /// Identificador �nico de la billetera.
        /// </summary>
        /// <example>1</example>
        public int WalletId { get; set; }
        /// <summary>
        /// Monto de la transferencia..
        /// </summary>
        /// <example>1</example>
        public decimal Amount { get; set; }
        /// <summary>
        /// Tipo de operaci�n (Cr�dito = 0 / D�bito = 1). 
        /// </summary>
        /// <example>0</example>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// Fecha del movimiento.
        /// </summary>
        /// <example>05/08/2025 11:11:11</example>
        public DateTime CreatedAt { get; set; }
    }
}