using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByDocumentIdAsync(string documentId);
        Task AddAsync(User user);
    }
}