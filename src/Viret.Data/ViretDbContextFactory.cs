using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Viret.Data;

public class ViretDbContextFactory : IDesignTimeDbContextFactory<ViretDbContext>
{
    public ViretDbContext CreateDbContext(string[] args)
    {
        var dbPath = Path.Combine(Path.GetTempPath(), "viret-design-time.db");
        var optionsBuilder = new DbContextOptionsBuilder<ViretDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        return new ViretDbContext(optionsBuilder.Options);
    }
}
