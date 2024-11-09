using System.Linq.Expressions;

namespace eDereva.Core.Interfaces
{
    public interface ISpecification<T>
    {
        // Expression representing the filtering criteria (e.g., for querying the database)
        Expression<Func<T, bool>> Criteria { get; }

        // Optionally, you can define sorting criteria
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }

        // Paging options
        int? Take { get; }
        int? Skip { get; }

        // Include related data (Eager Loading)
        Func<IQueryable<T>, IQueryable<T>> Include { get; }

        // A flag indicating whether to ignore soft deletes (if applicable)
        bool IgnoreSoftDeletes { get; }
    }
}
