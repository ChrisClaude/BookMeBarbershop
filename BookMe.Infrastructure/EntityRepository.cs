using System;
using BookMe.Application.Caching;
using BookMe.Application.Configurations;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookMe.Infrastructure;

/// <summary>
/// Represents the entity repository implementation
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
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
        _usingDistributedCache = appSettings.Value.DistributedCacheConfig.DistributedCacheType switch
        {
            DistributedCacheType.Redis => true,
            DistributedCacheType.SqlServer => true,
            _ => false
        };
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="getAllAsync">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    protected virtual async Task<IList<TEntity>> GetEntitiesAsync(Func<Task<IList<TEntity>>> getAllAsync, Func<IStaticCacheManager, CacheKey> getCacheKey)
    {
        if (getCacheKey == null)
            return await getAllAsync();

        //caching
        var cacheKey = getCacheKey(_staticCacheManager)
                       ?? _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<TEntity>.AllCacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, getAllAsync);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="getAllAsync">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    protected virtual async Task<IList<TEntity>> GetEntitiesAsync(Func<Task<IList<TEntity>>> getAllAsync, Func<IStaticCacheManager, Task<CacheKey>> getCacheKey)
    {
        if (getCacheKey == null)
            return await getAllAsync();

        //caching
        var cacheKey = await getCacheKey(_staticCacheManager)
                       ?? _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<TEntity>.AllCacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, getAllAsync);
    }

    /// <summary>
    /// Adds "deleted" filter to query which contains <see cref="ISoftDeletedEntity"/> entries, if its need
    /// </summary>
    /// <param name="query">Entity entries</param>
    /// <param name="includeDeleted">Whether to include deleted items</param>
    /// <returns>Entity entries</returns>
    protected virtual IQueryable<TEntity> AddDeletedFilter(IQueryable<TEntity> query, in bool includeDeleted)
    {
        if (includeDeleted)
            return query;

        if (typeof(TEntity).GetInterface(nameof(ISoftDeletedEntity)) == null)
            return query;

        return query.OfType<ISoftDeletedEntity>().Where(entry => !entry.Deleted).OfType<TEntity>();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the entity entry
    /// </summary>
    /// <param name="id">Entity entry identifier</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="BookMe.Core.Entities.ISoftDeletedEntity"/> entities)</param>
    /// <param name="useShortTermCache">Whether to use short term cache instead of static cache</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entry
    /// </returns>
    public virtual async Task<TEntity> GetByIdAsync(Guid? id, Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true, bool useShortTermCache = false)
    {
        if (!id.HasValue)
            return null;

        async Task<TEntity> getEntityAsync()
        {
            return await AddDeletedFilter(Table, includeDeleted).FirstOrDefaultAsync(entity => entity.Id == id);
        }

        if (getCacheKey == null)
            return await getEntityAsync();

        ICacheKeyService cacheKeyService = useShortTermCache ? _shortTermCacheManager : _staticCacheManager;

        //caching
        var cacheKey = getCacheKey(cacheKeyService)
                       ?? cacheKeyService.PrepareKeyForDefaultCache(CacheDefaults<TEntity>.ByIdCacheKey, id);

        if (useShortTermCache)
            return await _shortTermCacheManager.GetAsync(getEntityAsync, cacheKey);

        return await _staticCacheManager.GetAsync(cacheKey, getEntityAsync);
    }

    /// <summary>
    /// Get entity entries by identifiers
    /// </summary>
    /// <param name="ids">Entity entry identifiers</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public virtual async Task<IList<TEntity>> GetByIdsAsync(IList<Guid> ids, Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
        if (ids?.Any() != true)
            return new List<TEntity>();

        static IList<TEntity> sortByIdList(IList<Guid> listOfId, IDictionary<Guid, TEntity> entitiesById)
        {
            var sortedEntities = new List<TEntity>(listOfId.Count);

            foreach (var id in listOfId)
                if (entitiesById.TryGetValue(id, out var entry))
                    sortedEntities.Add(entry);

            return sortedEntities;
        }

        async Task<IList<TEntity>> getByIdsAsync(IList<Guid> listOfId, bool sort = true)
        {
            var query = AddDeletedFilter(Table, includeDeleted)
                .Where(entry => listOfId.Contains(entry.Id));

            return sort
                ? sortByIdList(listOfId, await query.ToDictionaryAsync(entry => entry.Id))
                : await query.ToListAsync();
        }

        if (getCacheKey == null)
            return await getByIdsAsync(ids);

        //caching
        var cacheKey = getCacheKey(_staticCacheManager);
        if (cacheKey == null && _usingDistributedCache)
            cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<TEntity>.ByIdsCacheKey, ids);
        if (cacheKey != null)
            return await _staticCacheManager.GetAsync(cacheKey, async () => await getByIdsAsync(ids));

        //if we are using an in-memory cache, we can optimize by caching each entity individually for maximum reusability.
        //with a distributed cache, the overhead would be too high.
        var cachedById = ids
            .Distinct()
            .Select(async id => await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<TEntity>.ByIdCacheKey, id),
                default(TEntity)))
            .Where(entity => entity != default)
            .ToDictionary(entity => entity.Id, entity => entity);
        var missingIds = ids.Except(cachedById.Keys).ToList();
        var missingEntities = missingIds.Count > 0 ? await getByIdsAsync(missingIds, false) : new List<TEntity>();

        foreach (var entity in missingEntities)
        {
            await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(CacheDefaults<TEntity>.ByIdCacheKey, entity.Id), entity);
            cachedById[entity.Id] = entity;
        }

        return sortByIdList(ids, cachedById);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="BookMe.Core.Entities.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public virtual async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
        async Task<IList<TEntity>> getAllAsync()
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            query = func != null ? func(query) : query;

            return await query.ToListAsync();
        }

        return await GetEntitiesAsync(getAllAsync, getCacheKey);
    }



    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public virtual async Task<IList<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
    {
        async Task<IList<TEntity>> getAllAsync()
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            query = func != null ? await func(query) : query;

            return await query.ToListAsync();
        }

        return await GetEntitiesAsync(getAllAsync, getCacheKey);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public virtual async Task<IList<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
        Func<ICacheKeyService, Task<CacheKey>> getCacheKey = null, bool includeDeleted = true)
    {
        async Task<IList<TEntity>> getAllAsync()
        {
            var query = AddDeletedFilter(Table, includeDeleted);
            query = func != null ? await func(query) : query;

            return await query.ToListAsync();
        }

        return await GetEntitiesAsync(getAllAsync, getCacheKey);
    }

    /// <summary>
    /// Get paged list of all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of entity entries
    /// </returns>
    public virtual async Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool includeDeleted = true)
    {
        var query = AddDeletedFilter(Table, includeDeleted);

        query = func != null ? func(query) : query;

        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    /// <summary>
    /// Get paged list of all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of entity entries
    /// </returns>
    public virtual async Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool includeDeleted = true)
    {
        var query = AddDeletedFilter(Table, includeDeleted);

        query = func != null ? await func(query) : query;

        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    /// <summary>
    /// Insert the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAsync(TEntity entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.InsertEntityAsync(entity);

        //event notification
        if (publishEvent)
            await _eventPublisher.EntityInsertedAsync(entity);
    }


    public virtual async Task InsertAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entities);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _context.BulkInsertEntitiesAsync(entities);
        transaction.Complete();

        if (!publishEvent)
            return;

        //event notification
        foreach (var entity in entities)
            await _eventPublisher.EntityInsertedAsync(entity);
    }






    public virtual async Task UpdateAsync(TEntity entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.UpdateEntityAsync(entity);

        //event notification
        if (publishEvent)
            await _eventPublisher.EntityUpdatedAsync(entity);
    }




    public virtual async Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (!entities.Any())
            return;

        await _context.UpdateEntitiesAsync(entities);

        //event notification
        if (!publishEvent)
            return;

        foreach (var entity in entities)
            await _eventPublisher.EntityUpdatedAsync(entity);
    }




    public virtual async Task DeleteAsync(TEntity entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity);

        switch (entity)
        {
            case ISoftDeletedEntity softDeletedEntity:
                softDeletedEntity.Deleted = true;
                await _context.UpdateEntityAsync(entity);
                break;

            default:
                await _context.DeleteEntityAsync(entity);
                break;
        }

        //event notification
        if (publishEvent)
            await _eventPublisher.EntityDeletedAsync(entity);
    }




    public virtual async Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (!entities.Any())
            return;

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (typeof(TEntity).GetInterface(nameof(ISoftDeletedEntity)) == null)
            await _context.BulkDeleteEntitiesAsync(entities);
        else
        {
            foreach (var entity in entities)
                ((ISoftDeletedEntity)entity).Deleted = true;

            await _context.UpdateEntitiesAsync(entities);
        }

        transaction.Complete();

        //event notification
        if (!publishEvent)
            return;

        foreach (var entity in entities)
            await _eventPublisher.EntityDeletedAsync(entity);
    }


    #endregion

    #region Properties

    /// <summary>
    /// Gets a table
    /// </summary>
    public virtual IQueryable<TEntity> Table => _context.Set<TEntity>();

    #endregion
}
