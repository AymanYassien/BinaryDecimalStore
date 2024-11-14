namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;

public interface ICategory : IRepository<Categorey>
{
    void update(Categorey modifiedCategory);
  

}