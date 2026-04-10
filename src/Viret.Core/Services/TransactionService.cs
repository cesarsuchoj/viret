using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Core.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Transaction>> GetTransactionsByFamilyAsync(int familyId)
        => _repository.GetByFamilyIdAsync(familyId);

    public async Task<Transaction> AddTransactionAsync(Transaction transaction)
    {
        if (transaction.Amount <= 0)
            throw new ArgumentException("Transaction amount must be greater than zero.", nameof(transaction));

        await _repository.AddAsync(transaction);
        return transaction;
    }

    public Task<decimal> GetBalanceAsync(int familyId)
        => _repository.GetBalanceByFamilyIdAsync(familyId);
}
