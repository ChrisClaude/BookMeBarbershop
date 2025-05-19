using System;
using BookMe.Application.Caching;
using BookMe.Application.Entities;

namespace BookMe.Application.Interfaces;

/// <summary>
/// Represents an entity repository
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    #region Methods

    Task<TEntity> GetByIdAsync(
        Guid? id,
        string[] includes = null,
        CacheKey cacheKey = null,
        bool includeDeleted = true
    );

    Task<IList<TEntity>> GetByIdsAsync(
        IList<Guid> ids,
        CacheKey cacheKey = null,
        bool includeDeleted = true
    );

    Task<IList<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        CacheKey cacheKey = null,
        bool includeDeleted = true
    );

    Task<IPagedList<TEntity>> GetAllPagedAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        bool getOnlyTotalCount = false,
        bool includeDeleted = true
    );

    Task InsertAsync(TEntity entity, bool publishEvent = true);

    Task InsertAsync(IList<TEntity> entities, bool publishEvent = true);

    Task UpdateAsync(TEntity entity, bool publishEvent = true);

    Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true);

    Task DeleteAsync(TEntity entity, bool publishEvent = true);

    Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true);

    #endregion
}
