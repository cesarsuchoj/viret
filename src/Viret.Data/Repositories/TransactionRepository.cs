using Microsoft.EntityFrameworkCore;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
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

        return transactions.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);
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
