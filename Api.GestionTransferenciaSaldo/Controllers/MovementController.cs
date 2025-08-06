using Application.DTOs.Movement;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IWalletService _walletService;

    public MovementController(IMovementService movementService, IWalletService walletService)
    {
        _movementService = movementService;
        _walletService = walletService;
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
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(MovementResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MovementResponse>> Create([FromBody] CreateMovementRequest request)
    {
        var userDocumentId = User.FindFirst("documentId")?.Value;

        if (string.IsNullOrEmpty(userDocumentId))
            return Forbid("Token inválido.");

        var wallet = await _walletService.GetByIdAsync(request.WalletId);

        if (wallet == null || wallet.DocumentId != userDocumentId)
            return Forbid("No puedes crear un movimiento en una billetera que no te pertenece.");

        var movement = request.ToEntity();
        var created = await _movementService.CreateAsync(movement);

        return CreatedAtAction(nameof(GetByWalletId), new { walletId = created.WalletId }, created.ToResponse());
    }

}