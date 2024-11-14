using System.Linq.Expressions;
using System.Runtime.InteropServices;
using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BinaryDecimalStore.BinaryStore.DataAccess.Repository;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly BinaryStoreDbContext _db;
    internal DbSet<T> _dbSet;

    public Repository(BinaryStoreDbContext db)
    {
        _db = db;
        this._dbSet = _db.Set<T>();
        
    }
    
    public IEnumerable<T> getAll()
    {
        IQueryable<T> query = _dbSet;
        return query; // unnecessary .ToList here
    }
    
    public IEnumerable<T> getAll(Expression<Func<T, object>>[] includes = null)
    {
        IQueryable<T> query = _dbSet;
        
        IQueryable<T> current = query;
        foreach (var include in includes)
        {
            current = current.Include(include);
        }
        return current;

        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        return query;
    }

    public T get(Expression<Func<T, bool>> filter)
    { 
        bool tracked = false;
        
        IQueryable<T> query;
        if(tracked)
            query = _dbSet; // AsTracking
        else
            query = _dbSet.AsNoTracking();
        
        query = query.Where(filter);

        return query.FirstOrDefault();
    }

    public T get(Expression<Func<T, bool>> filter, Expression<Func<T, object>>[] includes = null)
    {
        IQueryable<T> query = _dbSet;
        query = query.Where(filter);

        if (includes != null)
            
        {
            query = includes.Aggregate(query, (current, include) 
                => current.Include(include));
        }

        return query.FirstOrDefault();
    }

    public void add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void removeRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}