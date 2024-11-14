using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public interface IShoppingCart : IRepository<ShoppingCart>
{
    public void Update(ShoppingCart modifiedShoppingCart);
    
}