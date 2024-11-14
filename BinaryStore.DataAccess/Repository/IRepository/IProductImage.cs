using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public interface IProductImage : IRepository<ProductImage>
{
    void update(ProductImage modifiedP_Image);
}