using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface IIncomeRepository : IRepository<Income, int>
{
    Task<IEnumerable<Income>> GetByFamilyIdAsync(int familyId);
}
