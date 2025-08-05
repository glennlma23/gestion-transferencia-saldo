using Application.DTOs.Movement;
using Application.Interfaces;
using Application.Mappings;
using Microsoft.AspNetCore.Mvc;

namespace Api.GestionTransferenciaSaldo.Controllers;

/// <summary>
/// API para gestionar movimientos de billeteras.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MovementController : ControllerBase
{
    private readonly IMovementService _movementService;

    public MovementController(IMovementService movementService)
    {
        _movementService = movementService;
    }

    /// <summary>
    /// Obtiene todos los movimientos de una billetera por su ID.
    /// </summary>
    /// <param name="walletId">ID de la billetera</param>
    /// <returns>Lista de movimientos</returns>
    [HttpGet("wallet/{walletId}")]
    [ProducesResponseType(typeof(IEnumerable<MovementResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovementResponse>>> GetByWalletId(int walletId)
    {
        var movements = await _movementService.GetByWalletIdAsync(walletId);
        return Ok(movements.ToResponseList());
    }

    /// <summary>
    /// Crea un nuevo movimiento (crédito o débito).
    /// </summary>
    /// <param name="request">Datos del movimiento</param>
    /// <returns>Movimiento creado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MovementResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovementResponse>> Create([FromBody] CreateMovementRequest request)
    {
        var movement = request.ToEntity();
        var created = await _movementService.CreateAsync(movement);

        return CreatedAtAction(nameof(GetByWalletId), new { walletId = created.WalletId }, created.ToResponse());
    }
}