using System.Linq.Expressions;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public interface IRepository<T>
{
    IEnumerable<T> getAll();
    IEnumerable<T> getAll(Expression<Func<T, object>>[] includes = null);
    T get(Expression<Func<T, bool>> filter);
    
    T get(Expression<Func<T, bool>> filter, Expression<Func<T, object>>[] includes = null);
    
    void add(T entity);
    void remove(T entity);
    void removeRange(IEnumerable<T> entities);

}