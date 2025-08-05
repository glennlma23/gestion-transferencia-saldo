using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class MovementService : IMovementService
    {
        private readonly IMovementRepository _movementRepository;
        private readonly IWalletRepository _walletRepository;

        public MovementService(IMovementRepository movementRepository, IWalletRepository walletRepository)
        {
            _movementRepository = movementRepository;
            _walletRepository = walletRepository;
        }

        public async Task<IEnumerable<Movement>> GetByWalletIdAsync(int walletId)
        {
            return await _movementRepository.GetByWalletIdAsync(walletId);
        }

        public async Task<Movement> CreateAsync(Movement movement)
        {
            var wallet = await _walletRepository.GetByIdAsync(movement.WalletId);
            if (wallet is null)
                throw new ArgumentException("Billetera no encontrada, valide el WalletId ingresado.");

            if (movement.Type == Domain.Enums.MovementType.Debit && wallet.Balance < movement.Amount)
                throw new InvalidOperationException("Saldo insuficiente. Modifique el monto de la transferencia.");

            wallet.Balance += movement.Type == Domain.Enums.MovementType.Credit
                ? movement.Amount
                : -movement.Amount;

            await _movementRepository.AddAsync(movement);
            await _walletRepository.UpdateAsync(wallet);

            return movement;
        }
    }
}