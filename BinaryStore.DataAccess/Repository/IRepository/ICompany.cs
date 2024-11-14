namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;

public interface ICompany : IRepository<Company>
{
    void update(Company modifiedCompany);
}