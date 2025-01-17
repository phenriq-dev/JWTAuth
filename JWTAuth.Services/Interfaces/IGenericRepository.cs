using System.Linq.Expressions;

namespace JWTAuth.Core.Interfaces
{
    public interface IGenericRepository<TEntity, TModel>
         where TEntity : class
         where TModel : class
    {
        List<TModel> GetAll();
        List<TModel> FindBy(Expression<Func<TEntity, bool>> predicate);
        void Add(TModel entity);
        void Add(TEntity entity);
        int Add(TModel entity, bool returnId, string returnName);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        void Edit(TModel entity, Expression<Func<TEntity, object>> keyProperty);
        void Save();
    }
}
