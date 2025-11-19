using System.Reflection;

namespace NutritionTracker.Persistence.Contracts.Extensions;

/// <summary>
/// Reflection helper for setting private properties (useful for entity mapping)
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Set a private property value using reflection
    /// </summary>
    public static void SetPrivateProperty<T>(this object obj, string propertyName, T value)
    {
        var property = obj.GetType().GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            throw new InvalidOperationException($"Property '{propertyName}' not found or cannot be set on type '{obj.GetType().Name}'");
        }
    }

    /// <summary>
    /// Get a private field value using reflection
    /// </summary>
    public static T? GetPrivateField<T>(this object obj, string fieldName)
    {
        var field = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (field == null)
        {
            throw new InvalidOperationException($"Field '{fieldName}' not found on type '{obj.GetType().Name}'");
        }

        return (T?)field.GetValue(obj);
    }

    /// <summary>
    /// Set a private field value using reflection
    /// </summary>
    public static void SetPrivateField<T>(this object obj, string fieldName, T value)
    {
        var field = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (field == null)
        {
            throw new InvalidOperationException($"Field '{fieldName}' not found on type '{obj.GetType().Name}'");
        }

        field.SetValue(obj, value);
    }

    /// <summary>
    /// Create an instance using private constructor
    /// </summary>
    public static T CreateInstance<T>() where T : class
    {
        return (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
    }
}
