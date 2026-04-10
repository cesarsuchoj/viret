using Microsoft.EntityFrameworkCore;
using Viret.Core.Models;

namespace Viret.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Family> Families => Set<Family>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Description).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Amount).HasPrecision(18, 2);
            entity.HasOne(t => t.Family)
                  .WithMany(f => f.Transactions)
                  .HasForeignKey(t => t.FamilyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Family>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Name).IsRequired().HasMaxLength(100);
        });
    }
}
