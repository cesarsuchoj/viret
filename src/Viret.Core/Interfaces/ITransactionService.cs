using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetTransactionsByFamilyAsync(int familyId);
    Task<Transaction> AddTransactionAsync(Transaction transaction);
    Task<decimal> GetBalanceAsync(int familyId);
}
