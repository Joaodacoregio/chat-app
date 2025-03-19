using System.Linq.Expressions;

namespace chatApp.Server.Domain.Interfaces.Bases
{
    //Interface assincrona generica que implementa o padrão repository para desacoplar o DB
    public interface IBaseRepository<T> where T : class //T So pode ser classe
    {
        //Busca registro usando funções lambda (fiz para permitir buscas mais complexas)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
