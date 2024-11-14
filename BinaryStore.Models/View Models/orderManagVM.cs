namespace BinaryDecimalStore.Models.View_Models;

public class orderManagVM
{
    public orderHeader orderHeader { get; set; }
    public List<OrderDetail> orderDetails { get; set; }
    
}