using Application.DTOs.Movement;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings
{
    public static class MovementMappingExtensions
    {
        public static Movement ToEntity(this CreateMovementRequest request)
        {
            return new Movement
            {
                WalletId = request.WalletId,
                Amount = request.Amount,
                Type = (MovementType)request.Type,
                CreatedAt = DateTime.UtcNow.AddHours(-5)
            };
        }

        public static MovementResponse ToResponse(this Movement movement)
        {
            return new MovementResponse
            {
                Id = movement.Id,
                WalletId = movement.WalletId,
                Amount = movement.Amount,
                Type = movement.Type.ToString(),
                CreatedAt = movement.CreatedAt
            };
        }

        public static IEnumerable<MovementResponse> ToResponseList(this IEnumerable<Movement> movements)
        {
            return movements.Select(m => m.ToResponse());
        }
    }
}