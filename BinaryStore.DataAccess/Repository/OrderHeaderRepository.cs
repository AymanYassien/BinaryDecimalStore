using Binary.Utilities;
using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;
// using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;

public class OrderHeaderRepository : Repository<orderHeader>, IOrderHeader
{
    public readonly BinaryStoreDbContext _db;
    private IOrderHeader _orderHeaderImplementation;

    public OrderHeaderRepository(BinaryStoreDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(orderHeader newOrderHeader)
    {
        _db.OrderHeaders.Update(newOrderHeader);
    }

    public void UpdateOrderStatus(int orderId, string orderStatus, string? paymentStatus = null)
    {
        orderHeader order = _db.OrderHeaders.FirstOrDefault(o => o.Id == orderId);
        if (order != null )
        {
            order.OrderStatus = orderStatus;
            if (!String.IsNullOrEmpty(paymentStatus))
                order.PaymentStatus = paymentStatus;
        }
    }

    public void UpdateStripePayment(int orderId, string sessionId, string paymentIntentId)
    {
        orderHeader order = _db.OrderHeaders.FirstOrDefault(o => o.Id == orderId);

        if (!String.IsNullOrEmpty(sessionId))
        {
            order.SessionId = sessionId;
        }

        if (!String.IsNullOrEmpty(paymentIntentId))
        {
            order.PaymentIntentId = paymentIntentId;
            order.PaymentDate     = DateTime.Now;
        }
            
            
        
    }
}