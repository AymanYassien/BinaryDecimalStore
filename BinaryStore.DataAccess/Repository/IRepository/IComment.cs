using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public interface IComment: IRepository<Comment>
{
    public void Update(Comment modifiedComment);
}