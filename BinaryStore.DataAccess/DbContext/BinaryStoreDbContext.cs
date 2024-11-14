

using System.Data;
using BinaryDecimalStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using Microsoft.EntityFrameworkCore;
public class BinaryStoreDbContext :  IdentityDbContext<ExtendIdentity>
{
    public void ForceCloseConnection()
    {
        var connection = Database.GetDbConnection();
        if (connection.State == ConnectionState.Open)
        {
            connection.Close();
        }
    }
    public BinaryStoreDbContext(DbContextOptions<BinaryStoreDbContext> options) : base(options)
    {
        
    }
   

    public DbSet<Categorey>      Categoreies   { get; set; }
    public DbSet<Product>        Products      { get; set; }
    public DbSet<ExtendIdentity> AppUsers      { get; set; }
    public DbSet<Company>        Companies     { get; set; }
    public DbSet<ShoppingCart>   ShoppingCarts { get; set; }
    public DbSet<ProductImage>   ProductImages { get; set; }
    public DbSet<orderHeader>    OrderHeaders {get; set;}
    public DbSet<OrderDetail>   OrderDetails {get; set;}
    public DbSet<Comment>       Comments {get; set;}
                      
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            
        });
        
        modelBuilder.Entity<Categorey>().HasData(
            new { ID = 1, name = "iphone", Description = "a Modern Phones From Apple", DisplayOrder = 1 },
            new { ID = 2, name = "AirPods", Description = "a Modern HeadPhones From Apple", DisplayOrder = 3 },
            new { ID = 3, name = "MacBook", Description = "a Modern Laptop From Apple", DisplayOrder = 2 }
        );
        
        // case sensitive
        modelBuilder.Entity<Company>().HasData(
            new { ID = 1, Name = "Apple"  ,  State = "Middle", city = "Cairo", Address = "Group 63",
                Code = "1013", PhoneNumber = "01112774515", Imageurl = "N/A"},
            
            new { ID = 2, Name = "Samsung"  ,  State = "Korea", city = "Chang", Address = "Group 93",
                Code = "1803", PhoneNumber = "01112274565", Imageurl = "N/A"},
            
            new { ID = 3, Name = "Huawei"  ,  State = "China", city = "Bekin", Address = "Group 68",
                Code = "1022", PhoneNumber = "0151277595", Imageurl = "N/A"}
            
        );

         modelBuilder.Entity<Categorey>().HasIndex(c => c.DisplayOrder).IsUnique();

        modelBuilder.Entity<Product>().HasData(
            new {ID = 1, Name = "iPhone 15", Description = "The Best Phone in Century 21",
                price = 99.0, discountRatio = 1.0,  CategoryID = 1, Imageurl = "Emp"},
            new {ID = 2, Name = "iPhone 15 + ", Description = "The Best Phone in Century 21", 
                price = 89.9, discountRatio = 20.1, CategoryID = 1, Imageurl = "Emp"},
            new {ID = 3, Name = "iPhone 15 ++", Description = "The Best Phone in Century 21",
                price = 79.9, discountRatio = 1.0,  CategoryID = 1, Imageurl = "E"}
        );    
        
    }
}