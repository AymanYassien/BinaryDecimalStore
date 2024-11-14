using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;
using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;


namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

public class CategoryRepository : Repository<Categorey>, ICategory
{
    private readonly BinaryStoreDbContext _db;

    public CategoryRepository(BinaryStoreDbContext db) : base(db) 
        => _db = db;
   

    public void update(Categorey modifiedCategory) => _db.Categoreies.Update(modifiedCategory);

   
    
}