using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

using IRepository;
public class UnitOfWork : IUnitOfWork
{
    public ICategory Category         { get; private set; }
    public IProduct  Product          { get;  private set; }
    public ICompany Company           { get;   private set;}
    public IShoppingCart shoppingCart { get;   private set;}
    public IExtendIdentity AppIdentity        { get;   private set;}
    public IOrderHeader OrderHeader { get; private set; }
    public IOrderDetail OrderDetail { get; private set; }
    public IProductImage ProductImage { get; private set; }
    public IComment      Comment { get; private set;}


    private readonly BinaryStoreDbContext _db;
    
    public UnitOfWork(BinaryStoreDbContext db) 
    {
        _db = db;
        _db.ForceCloseConnection();

        
        Category     = new CategoryRepository(_db);
        Product      = new ProductRepo(_db);
        Company      = new CompanyRepository(_db);
        shoppingCart = new ShoppingCartRepository(_db);
        AppIdentity      = new ExtendIdentityRepository(_db);
        OrderDetail  = new OrderDetailsRepository(_db);
        OrderHeader  = new OrderHeaderRepository(_db);
        ProductImage = new ProductImageRepository(_db);
        Comment = new CommentRepo(_db);
    }

    public void save()     => _db.SaveChanges();
}