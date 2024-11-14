namespace BinaryDecimalStore.Models.View_Models;

public class shoppingCartVM
{
    public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
    public orderHeader orderHeader { get; set; }
    
    // public double Total { get; set; }
    
}