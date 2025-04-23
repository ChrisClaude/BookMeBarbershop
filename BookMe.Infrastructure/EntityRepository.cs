using System;
using BookMe.Application.Caching;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Configurations;
using BookMe.Application.Entities;
using BookMe.Application.Events;
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
    protected readonly ICacheManager _cacheManager;
    protected readonly bool _usingDistributedCache;

    #endregion

    #region Ctor

    public EntityRepository(IEventPublisher eventPublisher,
        BookMeContext context,
        ICacheManager cacheManager,
        IOptionsSnapshot<AppSettings> appSettings)
    {
        _eventPublisher = eventPublisher;
        _context = context;
        _cacheManager = cacheManager;
        _usingDistributedCache = appSettings?.Value.CacheConfig.CacheType switch
        {
            CacheType.Redis => true,
            CacheType.SqlServer => true,
            _ => false
        };
    }

    #endregion


    #region Methods

    public virtual async Task<TEntity> GetByIdAsync(Guid? id, CacheKey cacheKey = null, bool includeDeleted = true)
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

        if (cacheKey == null)
            return await GetEntityAsync();

        if (await _cacheManager.GetAsync(cacheKey, out TEntity entity))
            return entity;


        return await GetEntityAsync();
    }

    public virtual async Task<IList<TEntity>> GetByIdsAsync(IList<Guid> ids, CacheKey cacheKey = null, bool includeDeleted = true)
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

        if (cacheKey == null)
            return await GetEntitiesAsync();

        if (await _cacheManager.GetAsync(cacheKey, out IList<TEntity> entities))
            return entities;

        return await GetEntitiesAsync();
    }

    public virtual async Task<IList<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        CacheKey cacheKey = null,
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

        if (cacheKey == null)
            return await GetEntitiesAsync();

        if (await _cacheManager.GetAsync(cacheKey, out IList<TEntity> entities))
            return entities;

        return await GetEntitiesAsync();
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

        return await ToPagedListAsync(query, pageIndex, pageSize, getOnlyTotalCount);
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

    #region Utility methods
    protected virtual async Task<IPagedList<TEntity>> ToPagedListAsync(IQueryable<TEntity> query, int pageIndex, int pageSize, bool getOnlyTotalCount)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        // Get total count
        var totalCount = await query.CountAsync();

        // Return if only total count is requested
        if (getOnlyTotalCount)
            return new PagedList<TEntity>(new List<TEntity>(), pageIndex, pageSize, totalCount);

        // Adjust page size
        if (pageSize <= 0)
            pageSize = int.MaxValue;

        // Adjust page index
        if (pageIndex <= 0)
            pageIndex = 0;

        // Get paginated data
        var items = await query.Skip(pageIndex * pageSize)
                             .Take(pageSize)
                             .ToListAsync();

        return new PagedList<TEntity>(items, pageIndex, pageSize, totalCount);
    }
    #endregion

    #region Properties

    /// <summary>
    /// Gets a table
    /// </summary>
    private IQueryable<TEntity> Table => _context.Set<TEntity>();

    #endregion
}
