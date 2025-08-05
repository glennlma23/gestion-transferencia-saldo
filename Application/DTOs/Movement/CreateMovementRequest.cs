using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Movement
{
    public class CreateMovementRequest
    {
        /// <summary>
        /// Identificador �nico de la billetera.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "WalletId no puede ser vac�o.")]
        public int WalletId { get; set; }

        /// <summary>
        /// Monto de la transferencia.
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Amount no puede ser vac�o.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount debe ser mayor que 0.")]
        public decimal Amount { get; set; }
        /// <summary>
        /// Tipo de operaci�n (Cr�dito = 0 / D�bito = 1). 
        /// </summary>
        /// <example>0</example>
        [Required(ErrorMessage = "Type no puede ser vac�o.")]
        [Range(0, 1, ErrorMessage = "Type debe ser 0 (Cr�dito) o 1 (D�bito).")]
        public int Type { get; set; }
    }
}