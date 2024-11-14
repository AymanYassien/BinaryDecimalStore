using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public interface IOrderHeader : IRepository<orderHeader>
{
    public void Update(orderHeader newOrderHeader);
    public void UpdateOrderStatus(int orderId, string orderStatus, string? paymentStatus = null);
    public void UpdateStripePayment(int orderId, string sessionId, string paymentIntentId);
}