using BinaryDecimalStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BinaryDecimalStore.DbContext;

public class _BinaryStoreDbContext : Microsoft.EntityFrameworkCore.DbContext
{
  

   

    //public DbSet<Categorey> Categoreies { get; set; }
    public DbSet<Product>   Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new {ID = 1, Name = "iPhone 15", Description = "The Best Phone in Century 21", price = 999, discountRatio = 0.0},
            new {ID = 2, Name = "iPhone 15 + ", Description = "The Best Phone in Century 21", price = 899, discountRatio = 20},
            new {ID = 3, Name = "iPhone 15 ++", Description = "The Best Phone in Century 21", price = 799, discountRatio = 0.0}
        );            
                      
    }
}