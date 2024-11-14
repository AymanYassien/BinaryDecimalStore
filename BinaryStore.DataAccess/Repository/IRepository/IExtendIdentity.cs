using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public interface IExtendIdentity : IRepository<ExtendIdentity>
{
    public void update(ExtendIdentity appUser);

}