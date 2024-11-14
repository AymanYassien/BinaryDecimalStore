using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;
namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

public class CompanyRepository : Repository<Company>, ICompany
{
    private readonly BinaryStoreDbContext _db;

    public CompanyRepository(BinaryStoreDbContext db) : base(db) 
        => _db = db;
    public void update(Company modifiedCategory)
    {
        _db.Companies.Update(modifiedCategory);
    }
    
}