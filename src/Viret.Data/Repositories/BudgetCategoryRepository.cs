using Microsoft.EntityFrameworkCore;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Data.Repositories;

public class BudgetCategoryRepository : IBudgetCategoryRepository
{
    private readonly ViretDbContext _context;

    public BudgetCategoryRepository(ViretDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetCategory?> GetByIdAsync(int id)
        => await _context.BudgetCategories.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<BudgetCategory>> GetAllAsync()
        => await _context.BudgetCategories.ToListAsync();

    public async Task<IEnumerable<BudgetCategory>> GetByFamilyIdAsync(int familyId)
        => await _context.BudgetCategories.Where(c => c.FamilyId == familyId).ToListAsync();

    public async Task AddAsync(BudgetCategory entity)
    {
        await _context.BudgetCategories.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BudgetCategory entity)
    {
        _context.BudgetCategories.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.BudgetCategories.FindAsync(id);
        if (entity is not null)
        {
            _context.BudgetCategories.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
