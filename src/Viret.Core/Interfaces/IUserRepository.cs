using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface IUserRepository : IRepository<User, int>
{
    Task<User?> GetByEmailAsync(string email);
}
