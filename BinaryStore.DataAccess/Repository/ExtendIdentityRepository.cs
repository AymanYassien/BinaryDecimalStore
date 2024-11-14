using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

public class ExtendIdentityRepository : Repository<ExtendIdentity>, IExtendIdentity
{
    private readonly BinaryStoreDbContext _db;

    public ExtendIdentityRepository(BinaryStoreDbContext db) : base(db)
    {
        _db = db;
    }

    public void update(ExtendIdentity appUser)
    {
        _db.AppUsers.Update(appUser);
    }
}