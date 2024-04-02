using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace TelemetryStash.Database.Repositories;

public interface IDbRepository<TEntity>
{
    TEntity Add(TEntity entity);
    IEnumerable<TEntity> Add(IEnumerable<TEntity> entities);
    Task<List<TEntity>> All(Opts<TEntity>? opts = null);
    TEntity Upsert(TEntity entity);
    TEntity Update(TEntity entity);
    IEnumerable<TEntity> Update(IEnumerable<TEntity> entities);
    void Delete(TEntity entity);
    void StopTracking(TEntity entity);
    Task<int> SaveChangesAsync();
}

public abstract class RepositoryBase<TEntity>(TelemetryDbContext context) : IDbRepository<TEntity> where TEntity : class
{
    protected TelemetryDbContext Context => context;

    public TEntity Add(TEntity entity)
    {
        context.Set<TEntity>().Add(entity);
        return entity;
    }

    public IEnumerable<TEntity> Add(IEnumerable<TEntity> entities)
    {
        context.Set<TEntity>().AddRange(entities);
        return entities;
    }

    public async Task<List<TEntity>> All(Opts<TEntity>? opts = null)
    {
        return await Context
            .Set<TEntity>()
            .WithOptions(opts)
            .ToListAsync();
    }

    public TEntity Upsert(TEntity entity)
    {
        if(context.Entry(entity).State == EntityState.Detached)
        {
            Add(entity);
        } else
        {
            Update(entity);
        }

        return entity;
    }

    public TEntity Update(TEntity entity)
    {
        context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        return entities;
    }

    public void Delete(TEntity entity)
    {
        context.Remove(entity);
    }

    public void StopTracking(TEntity entity)
    {
        context.Entry(entity).State = EntityState.Detached;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}

public delegate void Opts<T>(IQueryableOptions<T> options);

public class IQueryableOptions<T>
{
    internal bool TrackEntities { get; private set; } = true;

    internal Expression<Func<T, object?>>[] IncludeEntities { get; private set; } = [];

    public IQueryableOptions<T> AsNoTracking()
    {
        TrackEntities = false;
        return this;
    }

    public IQueryableOptions<T> Include(params Expression<Func<T, object?>>[] expressions)
    {
        IncludeEntities = expressions;
        return this;
    }
}

internal static class IQueryableExtensions
{
    public static IQueryable<TEntity> WithOptions<TEntity>(this IQueryable<TEntity> source, Opts<TEntity>? options) where TEntity : class
    {
        if (options == null)
        {
            return source;
        }

        var opts = new IQueryableOptions<TEntity>();
        options(opts);

        source = opts.TrackEntities ? source.AsTracking() : source.AsNoTracking();

        foreach (var expression in opts.IncludeEntities)
        {
            source = source.Include(expression);
        }

        return source;
    }
}
