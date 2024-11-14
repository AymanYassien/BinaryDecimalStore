using BinaryDecimalStore.Models;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public interface IOrderDetail: IRepository<OrderDetail>
{
    public void Update(OrderDetail newOrderDetail);
}