using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface ITransactionRepository : IRepository<Transaction, int>
{
    Task<IEnumerable<Transaction>> GetByFamilyIdAsync(int familyId);
    Task<decimal> GetBalanceByFamilyIdAsync(int familyId);
}
