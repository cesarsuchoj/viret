using Microsoft.EntityFrameworkCore;
using Viret.Core.Models;

namespace Viret.Data;

public class ViretDbContext : DbContext
{
    public ViretDbContext(DbContextOptions<ViretDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Family> Families => Set<Family>();
    public DbSet<User> Users => Set<User>();
    public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();

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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<FamilyMember>(entity =>
        {
            entity.HasKey(fm => new { fm.UserId, fm.FamilyId });
            entity.Property(fm => fm.Role).IsRequired();
            entity.HasOne(fm => fm.User)
                .WithMany(u => u.FamilyMemberships)
                .HasForeignKey(fm => fm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(fm => fm.Family)
                .WithMany(f => f.Members)
                .HasForeignKey(fm => fm.FamilyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
