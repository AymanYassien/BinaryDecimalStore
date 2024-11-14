using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

public class OrderDetailsRepository : Repository<OrderDetail>, IOrderDetail
{
    public readonly BinaryStoreDbContext _db;
    public OrderDetailsRepository(BinaryStoreDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(OrderDetail newOrderDetail)
    {
        _db.OrderDetails.Update(newOrderDetail);
    }
}