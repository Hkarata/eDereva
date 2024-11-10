using System.Linq.Expressions;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<PaginatedResult<T>> GetAllAsync(PaginationParams pagination)
        {
            var query = _dbSet.AsNoTracking();
            var count = await query.CountAsync();

            var items = await query
                .OrderBy(SortPropertyHelper<T>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<T>(items, count, pagination);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            var result = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(object id)
        {
            return await _dbSet.FindAsync(id) != null;
        }
    }

    public static class SortPropertyHelper<T>
    {
        public static Expression<Func<T, object>> GetSortProperty(PaginationParams pagination)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression propertyExpression;

            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                // Default to first property or one ending in "Id"
                var propertyInfo = typeof(T).GetProperties()
                    .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                    ?? typeof(T).GetProperties().FirstOrDefault();

                if (propertyInfo == null)
                    throw new InvalidOperationException($"Type {typeof(T).Name} does not have any properties for sorting.");

                propertyExpression = Expression.Property(parameter, propertyInfo);
            }
            else
            {
                // Handle nested properties (e.g., "User.Name")
                var properties = pagination.SortBy.Split('.');
                propertyExpression = Expression.Property(parameter, properties[0]);

                for (int i = 1; i < properties.Length; i++)
                {
                    propertyExpression = Expression.Property(propertyExpression, properties[i]);
                }
            }

            var convertExpression = Expression.Convert(propertyExpression, typeof(object));
            return Expression.Lambda<Func<T, object>>(convertExpression, parameter);
        }
    }
}