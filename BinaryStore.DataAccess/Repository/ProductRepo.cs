using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

public class ProductRepo : Repository<Product>, IProduct
{
    private readonly BinaryStoreDbContext _db;

    public ProductRepo(BinaryStoreDbContext db) : base(db) 
        => _db = db;
    
    
    public void update(Product modifiedProduct)
    {
        _db.Products.Update(modifiedProduct);
    }
    
}