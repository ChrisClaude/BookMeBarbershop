using System;
using BookMe.Application.Caching;
using BookMe.Application.Configurations;
using BookMe.Application.Entities;
using BookMe.Application.Exceptions;
using BookMe.Application.Interfaces;
using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookMe.Infrastructure;

public partial class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    #region Fields

    protected readonly IEventPublisher _eventPublisher;
    protected readonly BookMeContext _context;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly bool _usingDistributedCache;

    #endregion

    #region Ctor

    public EntityRepository(IEventPublisher eventPublisher,
        BookMeContext context,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager,
        IOptionsSnapshot<AppSettings> appSettings)
    {
        _eventPublisher = eventPublisher;
        _context = context;
        _shortTermCacheManager = shortTermCacheManager;
        _staticCacheManager = staticCacheManager;
        _usingDistributedCache = appSettings?.Value.DistributedCacheConfig.DistributedCacheType switch
        {
            DistributedCacheType.Redis => true,
            DistributedCacheType.SqlServer => true,
            _ => false
        };
    }

    #endregion


    #region Methods

    public virtual async Task<TEntity> GetByIdAsync(Guid? id, Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true, bool useShortTermCache = false)
    {
        if (!id.HasValue)
            return null;

        async Task<TEntity> GetEntityAsync()
        {
            var query = Table;
            if (!includeDeleted && typeof(ISoftDeletedEntity).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(e => !((ISoftDeletedEntity)e).Deleted);
            }
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        if (getCacheKey == null)
            return await GetEntityAsync();

        ICacheKeyService cacheManager = useShortTermCache ? _shortTermCacheManager : _staticCacheManager;
        var cacheKey = getCacheKey(cacheManager);

        return await cacheManager.GetAsync(cacheKey, id.Value, GetEntityAsync);
    }

    public virtual async Task<IList<TEntity>> GetByIdsAsync(IList<Guid> ids, Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
        if (ids == null || !ids.Any())
            return new List<TEntity>();

        async Task<IList<TEntity>> GetEntitiesAsync()
        {
            var query = Table;
            if (!includeDeleted && typeof(ISoftDeletedEntity).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(e => !((ISoftDeletedEntity)e).Deleted);
            }
            return await query.Where(e => ids.Contains(e.Id)).ToListAsync();
        }

        if (getCacheKey == null)
            return await GetEntitiesAsync();

        var cacheKey = getCacheKey(_staticCacheManager);
        return await _staticCacheManager.GetAsync(cacheKey, ids, GetEntitiesAsync);
    }

    public virtual async Task<IList<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null,
        bool includeDeleted = true)
    {
        async Task<IList<TEntity>> GetEntitiesAsync()
        {
            var query = Table;
            if (!includeDeleted && typeof(ISoftDeletedEntity).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(e => !((ISoftDeletedEntity)e).Deleted);
            }

            query = func != null ? func(query) : query;

            return await query.ToListAsync();
        }

        if (getCacheKey == null)
            return await GetEntitiesAsync();

        var cacheKey = getCacheKey(_staticCacheManager);
        return await _staticCacheManager.GetAsync(cacheKey, async () => await GetEntitiesAsync());
    }

    public virtual async Task<IPagedList<TEntity>> GetAllPagedAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        bool getOnlyTotalCount = false,
        bool includeDeleted = true)
    {
        var query = Table;

        if (!includeDeleted && typeof(ISoftDeletedEntity).IsAssignableFrom(typeof(TEntity)))
        {
            query = query.Where(e => !((ISoftDeletedEntity)e).Deleted);
        }

        query = func != null ? func(query) : query;

        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    public virtual async Task InsertAsync(TEntity entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();

            if (publishEvent)
                await _eventPublisher.PublishAsync(new EntityInsertedEvent<TEntity>(entity));
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Error inserting entity of type {typeof(TEntity).Name}", ex);
        }
    }

    public virtual async Task InsertAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            if (publishEvent)
            {
                foreach (var entity in entities)
                {
                    await _eventPublisher.PublishAsync(new EntityInsertedEvent<TEntity>(entity));
                }
            }
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Error inserting entities of type {typeof(TEntity).Name}", ex);
        }
    }

    public virtual async Task UpdateAsync(TEntity entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();

            if (publishEvent)
                await _eventPublisher.PublishAsync(new EntityUpdatedEvent<TEntity>(entity));
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Error updating entity of type {typeof(TEntity).Name}", ex);
        }
    }

    public virtual async Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        try
        {
            _context.Set<TEntity>().UpdateRange(entities);
            await _context.SaveChangesAsync();

            if (publishEvent)
            {
                foreach (var entity in entities)
                {
                    await _eventPublisher.PublishAsync(new EntityUpdatedEvent<TEntity>(entity));
                }
            }
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Error updating entities of type {typeof(TEntity).Name}", ex);
        }
    }

    public virtual async Task DeleteAsync(TEntity entity, bool publishEvent = true)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            if (entity is ISoftDeletedEntity softDeletedEntity)
            {
                softDeletedEntity.Deleted = true;
                await UpdateAsync(entity, publishEvent);
            }
            else
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }

            if (publishEvent)
                await _eventPublisher.PublishAsync(new EntityDeletedEvent<TEntity>(entity));
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Error deleting entity of type {typeof(TEntity).Name}", ex);
        }
    }

    public virtual async Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            if (typeof(ISoftDeletedEntity).IsAssignableFrom(typeof(TEntity)))
            {
                foreach (var entity in entities)
                {
                    ((ISoftDeletedEntity)entity).Deleted = true;
                }
                await UpdateAsync(entities, publishEvent);
            }
            else
            {
                _context.Set<TEntity>().RemoveRange(entities);
                await _context.SaveChangesAsync();
            }

            if (publishEvent)
            {
                foreach (var entity in entities)
                {
                    await _eventPublisher.PublishAsync(new EntityDeletedEvent<TEntity>(entity));
                }
            }
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException($"Error deleting entities of type {typeof(TEntity).Name}", ex);
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a table
    /// </summary>
    public virtual IQueryable<TEntity> Table => _context.Set<TEntity>();

    #endregion
}
