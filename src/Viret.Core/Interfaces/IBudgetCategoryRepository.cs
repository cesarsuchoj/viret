using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface IBudgetCategoryRepository : IRepository<BudgetCategory, int>
{
    Task<IEnumerable<BudgetCategory>> GetByFamilyIdAsync(int familyId);
}
