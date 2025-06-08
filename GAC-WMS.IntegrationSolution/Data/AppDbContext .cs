using GAC_WMS.IntegrationSolution.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace GAC_WMS.IntegrationSolution.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

       
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }      
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }

        // Fluent API configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductCode)
                .IsUnique();

            modelBuilder.Entity<Customer>()
               .HasIndex(p => p.CustomerIdentifier)
               .IsUnique();

        }


    }
}
