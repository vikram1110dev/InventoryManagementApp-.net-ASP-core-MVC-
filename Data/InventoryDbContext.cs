using InventoryManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<StockBalance> StockBalances => Set<StockBalance>();
        public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Warehouse configuration
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Location configuration
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasIndex(e => new { e.WarehouseId, e.Aisle, e.Shelf, e.Bin }).IsUnique();
                entity.HasOne(e => e.Warehouse)
                    .WithMany(w => w.Locations)
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.HasIndex(e => e.Barcode);
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StockBalance configuration
            modelBuilder.Entity<StockBalance>(entity =>
            {
                entity.HasIndex(e => new { e.ProductId, e.LocationId }).IsUnique();
                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.StockBalances)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Location)
                    .WithMany(l => l.StockBalances)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // StockTransaction configuration
            modelBuilder.Entity<StockTransaction>(entity =>
            {
                entity.HasIndex(e => new { e.ProductId, e.CreatedAtUtc });

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.StockTransactions)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Location)
                    .WithMany(l => l.StockTransactions)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
