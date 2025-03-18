using chatApp.Server.Data.Context;
using chatApp.Server.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace chatApp.Server.Domain.Repositories.Bases
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly IAppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(IAppDbContext context)
        {
            _context = context;
            _dbSet = ((DbContext)context).Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
