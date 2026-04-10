using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface IFamilyMemberRepository
{
    Task<FamilyMember?> GetByIdsAsync(int userId, int familyId);
    Task<IEnumerable<Family>> GetFamiliesByUserIdAsync(int userId);
    Task AddAsync(FamilyMember familyMember);
    Task<bool> ExistsAsync(int userId, int familyId);
}
