using Application.DTOs.Wallet;
using Application.Interfaces;
using Application.Mappings;
using Microsoft.AspNetCore.Mvc;

namespace Api.GestionTransferenciaSaldo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    /// <summary>
    /// Método para obtener todas las billeteras existentes.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WalletResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WalletResponse>>> GetAll()
    {
        var wallets = await _walletService.GetAllAsync();
        return Ok(wallets.ToResponseList());
    }

    /// <summary>
    /// Método para obtener todas las billeteras de un determinado número de documento.
    /// </summary>
    /// <param name="documentId">Documento de identidad del propietario de la billetera.</param>
    [HttpGet("document/{documentId}")]
    [ProducesResponseType(typeof(IEnumerable<WalletResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WalletResponse>>> GetByDocumentId(string documentId)
    {
        var wallets = await _walletService.GetByDocumentIdAsync(documentId);
        return Ok(wallets.ToResponseList());
    }

    /// <summary>
    /// Método para obtener la billetera de un determinado número de documento y Id.
    /// </summary>
    /// <param name="id">Identificador único de la billetera.</param>
    /// <param name="documentId">Documento de identidad del propietario de la billetera.</param>
    [HttpGet("{id}/document/{documentId}")]
    [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletResponse>> GetByIdAndDocumentId(int id, string documentId)
    {
        var wallet = await _walletService.GetByIdAndDocumentIdAsync(id, documentId);
        if (wallet is null)
            return NotFound();

        return Ok(wallet.ToResponse());
    }

    /// <summary>
    /// Método para registrar una nueva billetera.
    /// </summary>
    /// <param name="request">Datos de la billetera a crear.</param>
    [HttpPost]
    [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WalletResponse>> Create([FromBody] CreateWalletRequest request)
    {
        var wallet = request.ToEntity();
        var created = await _walletService.CreateAsync(wallet);
        return CreatedAtAction(nameof(GetByIdAndDocumentId),
            new { id = created.Id, documentId = created.DocumentId },
            created.ToResponse());
    }

    /// <summary>
    /// Método para actualizar una billetera con un determinado Id.
    /// </summary>
    /// <param name="id">Identificador único de la billetera.</param>
    /// <param name="request">Datos actualizados de la billetera.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWalletRequest request)
    {
        var existing = await _walletService.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        existing.UpdateFromRequest(request);
        await _walletService.UpdateAsync(existing);

        return NoContent();
    }

    /// <summary>
    /// Método para eliminar una billetera con un determinado Id.
    /// </summary>
    /// <param name="id">Identificador único de la billetera.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _walletService.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        await _walletService.DeleteAsync(id);
        return NoContent();
    }
}
