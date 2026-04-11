using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface IExpenseRepository : IRepository<Expense, int>
{
    Task<IEnumerable<Expense>> GetByFamilyIdAsync(int familyId);
}
