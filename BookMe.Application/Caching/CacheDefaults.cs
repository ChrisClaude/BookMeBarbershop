using System;
using BookMe.Application.Entities;

namespace BookMe.Application.Caching;

public static class CacheDefaults<TEntity> where TEntity : BaseEntity
{
  /// <summary>
  /// Gets an entity type name used in cache keys
  /// </summary>
  public static string EntityTypeName => typeof(TEntity).Name.ToLowerInvariant();

  /// <summary>
  /// Gets a key for caching entity by identifier
  /// </summary>
  /// <remarks>
  /// {0} : entity id
  /// </remarks>
  public static CacheKey ByIdCacheKey => new($"BookMe.{EntityTypeName}.byid.{{0}}");

  /// <summary>
  /// Gets a key for caching entities by identifiers
  /// </summary>
  /// <remarks>
  /// {0} : entity ids
  /// </remarks>
  public static CacheKey ByIdsCacheKey => new($"BookMe.{EntityTypeName}.byids.{{0}}");

  /// <summary>
  /// Gets a key for caching all entities
  /// </summary>
  public static CacheKey AllCacheKey => new($"BookMe.{EntityTypeName}.all.");

  /// <summary>
  /// Gets a key pattern to clear cache
  /// </summary>
  public static string Prefix => $"BookMe.{EntityTypeName}.";

  /// <summary>
  /// Gets a key pattern to clear cache
  /// </summary>
  public static string ByIdPrefix => $"BookMe.{EntityTypeName}.byid.";

  /// <summary>
  /// Gets a key pattern to clear cache
  /// </summary>
  public static string ByIdsPrefix => $"BookMe.{EntityTypeName}.byids.";

  /// <summary>
  /// Gets a key pattern to clear cache
  /// </summary>
  public static string AllPrefix => $"BookMe.{EntityTypeName}.all.";
}
