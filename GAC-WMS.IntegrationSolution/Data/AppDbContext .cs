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
            modelBuilder.Entity<Product>().HasIndex(p => p.ProductCode).IsUnique();

            modelBuilder.Entity<Customer>() .HasIndex(p => p.CustomerIdentifier).IsUnique();
            modelBuilder.Entity<PurchaseOrder>() .HasIndex(p => p.OrderId).IsUnique();
            modelBuilder.Entity<SalesOrder>() .HasIndex(p => p.OrderId).IsUnique();


            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Customer)
                .WithMany(c => c.PurchaseOrders)
                .HasForeignKey(po => po.CustomerIdentifier)
                .HasPrincipalKey(c => c.CustomerIdentifier);

            modelBuilder.Entity<SalesOrder>()
            .HasOne(so => so.Customer)
            .WithMany(c => c.SalesOrders)
            .HasForeignKey(so => so.CustomerIdentifier)
            .HasPrincipalKey(c => c.CustomerIdentifier);


        }


    }
}
