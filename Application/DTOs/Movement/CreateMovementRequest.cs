using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Movement
{
    public class CreateMovementRequest
    {
        /// <summary>
        /// Identificador único de la billetera.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "WalletId no puede ser vacío.")]
        public int WalletId { get; set; }

        /// <summary>
        /// Monto de la transferencia.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Amount no puede ser vacío.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount debe ser mayor que 0.")]
        public decimal Amount { get; set; }
        /// <summary>
        /// Tipo de operación (Crédito = 0 / Débito = 1). 
        /// </summary>
        /// <example>0</example>
        [Required(ErrorMessage = "Type no puede ser vacío.")]
        [Range(0, 1, ErrorMessage = "Type debe ser 0 (Crédito) o 1 (Débito).")]
        public int Type { get; set; }
    }
}