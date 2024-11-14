using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;
using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;


namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

public class ProductImageRepository : Repository<ProductImage>, IProductImage
{
    private readonly BinaryStoreDbContext _db;

    public ProductImageRepository(BinaryStoreDbContext db) : base(db) 
        => _db = db;
   

    public void update(ProductImage modifiedP_Image) => _db.ProductImages.Update(modifiedP_Image);

   
    
}