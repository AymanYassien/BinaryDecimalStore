using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public interface IProduct : IRepository<Product>
{
    public void update(Product modifiedProduct);
}