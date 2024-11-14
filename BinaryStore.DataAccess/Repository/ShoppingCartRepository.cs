using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCart
{
    private readonly BinaryStoreDbContext _db;

    public ShoppingCartRepository(BinaryStoreDbContext db) : base(db) 
        => _db = db;
    
    
    public void Update(ShoppingCart modifiedShoppingCart)
    {
        _db.ShoppingCarts.Update(modifiedShoppingCart);
    }
}