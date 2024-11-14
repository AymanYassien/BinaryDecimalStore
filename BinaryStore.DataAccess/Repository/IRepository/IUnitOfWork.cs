namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    public ICategory Category { get; }
    public IProduct  Product { get; }
    
    public ICompany  Company { get; }
    
    public IShoppingCart shoppingCart { get; }
    public IExtendIdentity AppIdentity { get; }
    public IOrderHeader OrderHeader { get; }
    public IOrderDetail OrderDetail { get; }
    public IProductImage ProductImage { get; }
    public IComment Comment { get; }

    public void save();
}