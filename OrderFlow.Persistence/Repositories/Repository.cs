using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Persistence.Context;
using System.Linq.Expressions;

namespace OrderFlow.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly OrderFlowDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(OrderFlowDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await _dbSet.AddRangeAsync(entities);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void UpdateRange(IEnumerable<T> entities)
            => _dbSet.UpdateRange(entities);

        public void Remove(T entity)
            => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities)
            => _dbSet.RemoveRange(entities);

        public async Task<bool> CompleteAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}
