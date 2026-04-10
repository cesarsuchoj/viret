using Microsoft.EntityFrameworkCore;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Data.Repositories;

public class FamilyRepository : IFamilyRepository
{
    private readonly AppDbContext _context;

    public FamilyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Family?> GetByIdAsync(int id)
        => await _context.Families.Include(f => f.Transactions).FirstOrDefaultAsync(f => f.Id == id);

    public async Task<IEnumerable<Family>> GetAllAsync()
        => await _context.Families.ToListAsync();

    public async Task AddAsync(Family entity)
    {
        await _context.Families.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Family entity)
    {
        _context.Families.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Families.FindAsync(id);
        if (entity is not null)
        {
            _context.Families.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
