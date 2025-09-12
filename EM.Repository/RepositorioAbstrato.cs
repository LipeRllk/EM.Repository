using EM.Domain.Interface;
using System.Linq.Expressions;

namespace EM.Repository
{
    /// <summary>
    /// Repositório abstrato genérico que implementa operações CRUD básicas
    /// Demonstra o uso de Generics, LINQ e Expression trees
    /// </summary>
    /// <typeparam name="T">Tipo da entidade que deve implementar IEntidade</typeparam>
    public abstract class RepositorioAbstrato<T> where T : class
    {
        protected readonly IDbConnectionFactory _connectionFactory;

        protected RepositorioAbstrato(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Adiciona uma nova entidade no repositório
        /// </summary>
        /// <param name="objeto">Entidade a ser adicionada</param>
        public abstract void Add(T objeto);

        /// <summary>
        /// Remove uma entidade do repositório
        /// </summary>
        /// <param name="objeto">Entidade a ser removida</param>
        public abstract void Remove(T objeto);

        /// <summary>
        /// Atualiza uma entidade no repositório
        /// </summary>
        /// <param name="objeto">Entidade a ser atualizada</param>
        public abstract void Update(T objeto);

        /// <summary>
        /// Retorna todas as entidades do repositório
        /// </summary>
        /// <returns>Lista enumerável de entidades</returns>
        public abstract IEnumerable<T> GetAll();

        /// <summary>
        /// Busca entidades baseadas em um predicado usando LINQ
        /// </summary>
        /// <param name="predicate">Expressão lambda para filtrar os resultados</param>
        /// <returns>Lista enumerável de entidades filtradas</returns>
        public abstract IEnumerable<T> Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Busca uma única entidade baseada em um predicado
        /// </summary>
        /// <param name="predicate">Expressão lambda para encontrar a entidade</param>
        /// <returns>Entidade encontrada ou null</returns>
        public virtual T? GetSingle(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Verifica se existe alguma entidade que satisfaça o predicado
        /// </summary>
        /// <param name="predicate">Expressão lambda para verificação</param>
        /// <returns>True se existe, False caso contrário</returns>
        public virtual bool Any(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate).Any();
        }

        /// <summary>
        /// Conta quantas entidades satisfazem o predicado
        /// </summary>
        /// <param name="predicate">Expressão lambda para filtrar</param>
        /// <returns>Número de entidades</returns>
        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate).Count();
        }
    }
}