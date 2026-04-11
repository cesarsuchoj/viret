using Microsoft.EntityFrameworkCore;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Data.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ViretDbContext _context;

    public ExpenseRepository(ViretDbContext context)
    {
        _context = context;
    }

    public async Task<Expense?> GetByIdAsync(int id)
        => await _context.Expenses.Include(e => e.BudgetCategory).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<IEnumerable<Expense>> GetAllAsync()
        => await _context.Expenses.Include(e => e.BudgetCategory).ToListAsync();

    public async Task<IEnumerable<Expense>> GetByFamilyIdAsync(int familyId)
        => await _context.Expenses.Where(e => e.FamilyId == familyId).ToListAsync();

    public async Task AddAsync(Expense entity)
    {
        await _context.Expenses.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Expense entity)
    {
        _context.Expenses.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Expenses.FindAsync(id);
        if (entity is not null)
        {
            _context.Expenses.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
