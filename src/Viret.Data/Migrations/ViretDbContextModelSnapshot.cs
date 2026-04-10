using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Viret.Data;

#nullable disable

namespace Viret.Data.Migrations;

[DbContext(typeof(ViretDbContext))]
partial class ViretDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

        modelBuilder.Entity("Viret.Core.Models.Family", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("Families");
            });

        modelBuilder.Entity("Viret.Core.Models.Transaction", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<decimal>("Amount")
                    .HasPrecision(18, 2)
                    .HasColumnType("TEXT");

                b.Property<DateTime>("Date")
                    .HasColumnType("TEXT");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("TEXT");

                b.Property<int>("FamilyId")
                    .HasColumnType("INTEGER");

                b.Property<int>("Type")
                    .HasColumnType("INTEGER");

                b.HasKey("Id");

                b.HasIndex("FamilyId");

                b.ToTable("Transactions");
            });

        modelBuilder.Entity("Viret.Core.Models.Transaction", b =>
            {
                b.HasOne("Viret.Core.Models.Family", "Family")
                    .WithMany("Transactions")
                    .HasForeignKey("FamilyId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Family");
            });

        modelBuilder.Entity("Viret.Core.Models.Family", b =>
            {
                b.Navigation("Transactions");
            });
#pragma warning restore 612, 618
    }
}
