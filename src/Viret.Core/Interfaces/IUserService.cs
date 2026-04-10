using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface IUserService
{
    Task<User> RegisterAsync(string name, string email, string password);
    Task<User> LoginAsync(string email, string password);
    Task<IEnumerable<Family>> GetFamiliesForUserAsync(int userId);
    Task AddUserToFamilyAsync(int userId, int familyId, FamilyRole role = FamilyRole.Member);
    Task<bool> UserHasAccessToFamilyAsync(int userId, int familyId);
}
