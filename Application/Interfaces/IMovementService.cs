using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMovementService
    {
        Task<IEnumerable<Movement>> GetByWalletIdAsync(int walletId);
        Task<Movement> CreateAsync(Movement movement);
    }
}