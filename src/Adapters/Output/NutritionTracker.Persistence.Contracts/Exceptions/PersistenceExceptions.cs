namespace NutritionTracker.Persistence.Contracts.Exceptions;

/// <summary>
/// Base exception for persistence-specific errors
/// </summary>
public abstract class PersistenceException : Exception
{
    public string? EntityType { get; }
    public object? EntityId { get; }

    protected PersistenceException(string message) : base(message) { }

    protected PersistenceException(string message, Exception innerException)
        : base(message, innerException) { }

    protected PersistenceException(string message, string? entityType, object? entityId)
        : base(message)
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}

/// <summary>
/// Exception thrown when an entity is not found in the database
/// </summary>
public class EntityNotFoundException : PersistenceException
{
    public EntityNotFoundException(string entityType, object entityId)
        : base($"{entityType} with identifier '{entityId}' was not found.", entityType, entityId)
    {
    }

    public EntityNotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when a database constraint is violated
/// </summary>
public class DatabaseConstraintException : PersistenceException
{
    public string ConstraintName { get; }

    public DatabaseConstraintException(string constraintName, string message)
        : base(message)
    {
        ConstraintName = constraintName;
    }

    public DatabaseConstraintException(string constraintName, string message, Exception innerException)
        : base(message, innerException)
    {
        ConstraintName = constraintName;
    }
}

/// <summary>
/// Exception thrown when a database operation fails
/// </summary>
public class DatabaseOperationException : PersistenceException
{
    public DatabaseOperationException(string message) : base(message) { }

    public DatabaseOperationException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a concurrency conflict occurs
/// </summary>
public class ConcurrencyException : PersistenceException
{
    public ConcurrencyException(string entityType, object entityId)
        : base($"The {entityType} with identifier '{entityId}' was modified by another user.", entityType, entityId)
    {
    }

    public ConcurrencyException(string message) : base(message) { }
}
