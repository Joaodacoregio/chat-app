using chatApp.Server.Data.Context;
using chatApp.Server.Domain.Interfaces.Bases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace chatApp.Server.Application.Bases
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly IAppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(IAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = ((DbContext)context).Set<T>() ?? throw new InvalidOperationException("DbSet não encontrado para a entidade.");
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var result = await _dbSet.ToListAsync();
            return result ?? new List<T>(); // Retorna lista vazia se null (por segurança)
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID inválido.");
            }

            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), "Predicate não pode ser nulo.");
            }

            var result = await _dbSet.Where(predicate).ToListAsync();
            return result ?? new List<T>();
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entidade não pode ser nula.");
            }

            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entidade não pode ser nula.");
            }

            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entidade não pode ser nula.");
            }

            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            var changes = await _context.SaveChangesAsync();
            if (changes == 0)
            {
                throw new InvalidOperationException("Nenhuma alteração foi salva no banco.");
            }
        }
    }
}
