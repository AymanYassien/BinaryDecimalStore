using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

public class CommentRepo: Repository<Comment>, IComment
{
    private readonly BinaryStoreDbContext _db;

    
    public CommentRepo(BinaryStoreDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Comment modifiedComment)
    {
        _db.Comments.Update(modifiedComment);
    }
}