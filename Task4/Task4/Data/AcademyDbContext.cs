using Microsoft.EntityFrameworkCore;
using Task4.Models;

namespace AcademyGraduationSupplyManagement.Data;

public class AcademyDbContext : DbContext
{
    public AcademyDbContext(DbContextOptions<AcademyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Offer> Offers => Set<Offer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasMany(category => category.Suppliers)
            .WithOne(supplier => supplier.Category)
            .HasForeignKey(supplier => supplier.CategoryId);

        modelBuilder.Entity<Supplier>()
            .HasOne(supplier => supplier.Offer)
            .WithOne(offer => offer.Supplier)
            .HasForeignKey<Offer>(offer => offer.SupplierId);

        base.OnModelCreating(modelBuilder);
    }
}