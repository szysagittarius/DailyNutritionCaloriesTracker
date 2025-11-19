namespace NutritionTracker.Persistence.Contracts.Specifications;

/// <summary>
/// Specification pattern for building complex queries
/// </summary>
public interface ISpecification<T>
{
    /// <summary>
    /// Criteria expression for filtering
    /// </summary>
    System.Linq.Expressions.Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Include expressions for eager loading
    /// </summary>
    List<System.Linq.Expressions.Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Include strings for eager loading (navigation properties)
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Order by expression
    /// </summary>
    System.Linq.Expressions.Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Order by descending expression
    /// </summary>
    System.Linq.Expressions.Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Pagination: Skip count
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// Pagination: Take count
    /// </summary>
    int Take { get; }

    /// <summary>
    /// Enable pagination
    /// </summary>
    bool IsPagingEnabled { get; }
}

/// <summary>
/// Base specification with common functionality
/// </summary>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    protected BaseSpecification(System.Linq.Expressions.Expression<Func<T, bool>>? criteria = null)
    {
        Criteria = criteria;
    }

    public System.Linq.Expressions.Expression<Func<T, bool>>? Criteria { get; }
    public List<System.Linq.Expressions.Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public System.Linq.Expressions.Expression<Func<T, object>>? OrderBy { get; private set; }
    public System.Linq.Expressions.Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int Skip { get; private set; }
    public int Take { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    protected void AddInclude(System.Linq.Expressions.Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected void ApplyOrderBy(System.Linq.Expressions.Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void ApplyOrderByDescending(System.Linq.Expressions.Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}
