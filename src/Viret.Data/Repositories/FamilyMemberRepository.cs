using Microsoft.EntityFrameworkCore;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Data.Repositories;

public class FamilyMemberRepository : IFamilyMemberRepository
{
    private readonly ViretDbContext _context;

    public FamilyMemberRepository(ViretDbContext context)
    {
        _context = context;
    }

    public async Task<FamilyMember?> GetByIdsAsync(int userId, int familyId)
        => await _context.FamilyMembers
            .Include(fm => fm.Family)
            .Include(fm => fm.User)
            .FirstOrDefaultAsync(fm => fm.UserId == userId && fm.FamilyId == familyId);

    public async Task<IEnumerable<Family>> GetFamiliesByUserIdAsync(int userId)
        => await _context.FamilyMembers
            .Where(fm => fm.UserId == userId)
            .Select(fm => fm.Family!)
            .ToListAsync();

    public async Task AddAsync(FamilyMember familyMember)
    {
        await _context.FamilyMembers.AddAsync(familyMember);
        await _context.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(int userId, int familyId)
        => _context.FamilyMembers.AnyAsync(fm => fm.UserId == userId && fm.FamilyId == familyId);
}
