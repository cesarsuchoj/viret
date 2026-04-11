using Microsoft.EntityFrameworkCore;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Data.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly ViretDbContext _context;

    public IncomeRepository(ViretDbContext context)
    {
        _context = context;
    }

    public async Task<Income?> GetByIdAsync(int id)
        => await _context.Incomes.Include(i => i.BudgetCategory).FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Income>> GetAllAsync()
        => await _context.Incomes.Include(i => i.BudgetCategory).ToListAsync();

    public async Task<IEnumerable<Income>> GetByFamilyIdAsync(int familyId)
        => await _context.Incomes.Where(i => i.FamilyId == familyId).ToListAsync();

    public async Task AddAsync(Income entity)
    {
        await _context.Incomes.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Income entity)
    {
        _context.Incomes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Incomes.FindAsync(id);
        if (entity is not null)
        {
            _context.Incomes.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
