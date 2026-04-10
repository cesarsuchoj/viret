using Microsoft.EntityFrameworkCore;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ViretDbContext _context;

    public TransactionRepository(ViretDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(int id)
        => await _context.Transactions.Include(t => t.Family).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Transaction>> GetAllAsync()
        => await _context.Transactions.Include(t => t.Family).ToListAsync();

    public async Task<IEnumerable<Transaction>> GetByFamilyIdAsync(int familyId)
        => await _context.Transactions.Where(t => t.FamilyId == familyId).ToListAsync();

    public async Task<decimal> GetBalanceByFamilyIdAsync(int familyId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.FamilyId == familyId)
            .ToListAsync();

        var income = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var expense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        return income - expense;
    }

    public async Task AddAsync(Transaction entity)
    {
        await _context.Transactions.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction entity)
    {
        _context.Transactions.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Transactions.FindAsync(id);
        if (entity is not null)
        {
            _context.Transactions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
