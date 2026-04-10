using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Core.Services;

public class FamilyService : IFamilyService
{
    private readonly IFamilyRepository _repository;

    public FamilyService(IFamilyRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Family>> GetAllFamiliesAsync()
        => _repository.GetAllAsync();

    public async Task<Family> CreateFamilyAsync(Family family)
    {
        if (string.IsNullOrWhiteSpace(family.Name))
            throw new ArgumentException("Family name cannot be empty.", nameof(family));

        await _repository.AddAsync(family);
        return family;
    }
}
