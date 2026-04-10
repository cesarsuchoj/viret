using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface IFamilyService
{
    Task<IEnumerable<Family>> GetAllFamiliesAsync();
    Task<Family> CreateFamilyAsync(Family family);
}
