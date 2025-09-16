using System.Linq.Expressions;

namespace EM.Repository
{

    public abstract class RepositorioAbstrato<T>(IDbConnectionFactory connectionFactory) where T : class
    {
        protected readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public abstract void Add(T objeto);


        public abstract void Remove(T objeto);


        public abstract void Update(T objeto);


        public abstract IEnumerable<T> GetAll();


        public abstract IEnumerable<T> Get(Expression<Func<T, bool>> predicate);


        public virtual T? GetSingle(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate).FirstOrDefault();
        }


        public virtual bool Any(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate).Any();
        }

        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate).Count();
        }
    }
}