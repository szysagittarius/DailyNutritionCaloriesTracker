using NutritionTracker.Domain.Entities;

namespace NutritionTracker.Persistence.Contracts.Common;

/// <summary>
/// Base interface for entities that can be mapped between persistence and domain models
/// </summary>
public interface IEntityMapper<TDomain, TPersistence>
    where TDomain : class
    where TPersistence : class
{
    /// <summary>
    /// Convert from persistence entity to domain entity
    /// </summary>
    TDomain ToDomain(TPersistence entity);

    /// <summary>
    /// Convert from domain entity to persistence entity
    /// </summary>
    TPersistence ToEntity(TDomain domain);
}

/// <summary>
/// Helper class for entity mapping operations
/// </summary>
public static class EntityMapperHelper
{
    /// <summary>
    /// Map collection from persistence to domain
    /// </summary>
    public static IEnumerable<TDomain> ToDomain<TDomain, TPersistence>(
        this IEntityMapper<TDomain, TPersistence> mapper,
        IEnumerable<TPersistence> entities)
        where TDomain : class
        where TPersistence : class
    {
        return entities.Select(mapper.ToDomain);
    }

    /// <summary>
    /// Map collection from domain to persistence
    /// </summary>
    public static IEnumerable<TPersistence> ToEntity<TDomain, TPersistence>(
        this IEntityMapper<TDomain, TPersistence> mapper,
        IEnumerable<TDomain> domains)
        where TDomain : class
        where TPersistence : class
    {
        return domains.Select(mapper.ToEntity);
    }
}
