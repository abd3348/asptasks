using Microsoft.EntityFrameworkCore;
using CafeteriaCodeFirst.Models;

namespace CafeteriaCodeFirst.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(p => p.Product)
                .WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}