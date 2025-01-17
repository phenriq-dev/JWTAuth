using AutoMapper;
using JWTAuth.Core.Interfaces;
using JWTAuth.Db.DataContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

namespace JWTAuth.Core.Services
{
    public class GenericRepository<TEntity, TModel> : Profile, IGenericRepository<TEntity, TModel>
           where TEntity : class
           where TModel : class
    {
        private DataContext _db;

        public GenericRepository(DataContext db) {
            _db = db;
        }
        public virtual List<TModel> GetAll()
        {
            var ent = _db.Set<TEntity>();
            var query = ent.ToList();

            var list = MapToModelList<TEntity, TModel>(query).ToList();
            return list;
        }

        public List<TModel> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            var ent = _db.Set<TEntity>();
            var query = ent.Where(predicate).ToList();

            var list = MapToModelList<TEntity, TModel>(query).ToList();
            return list;
        }

        public TModel SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            var ent = _db.Set<TEntity>();
            var query = ent.SingleOrDefault(predicate);

            if (query == null)
                return null;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<TEntity, TModel>());
            var mapper = new Mapper(config);
            var item = mapper.Map<TEntity, TModel>(query);

            return item;
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            var result = _db.Set<TEntity>().Any(predicate);
            return result;
        }

        public virtual void Add(TModel entity)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TModel, TEntity>());
            var mapper = new Mapper(config);
            var item = mapper.Map<TModel, TEntity>(entity);

            _db.Set<TEntity>().Add(item);
            Save();
        }

        public virtual int Add(TModel entity, bool returnId, string returnName)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TModel, TEntity>());
            var mapper = new Mapper(config);
            var item = mapper.Map<TModel, TEntity>(entity);

            _db.Set<TEntity>().Add(item);
            Save();
            return returnId ? (int)item.GetType().GetProperty(returnName).GetValue(item, null) : 0;
        }

        public virtual void Add(TEntity entity)
        {
            _db.Set<TEntity>().Add(entity);
            Save();
        }

        public virtual TEntity AddGetId(TModel entity)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TModel, TEntity>());
            var mapper = new Mapper(config);
            var item = mapper.Map<TModel, TEntity>(entity);

            _db.Set<TEntity>().Add(item);
            Save();
            return item;
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            _db.Set<TEntity>().RemoveRange(_db.Set<TEntity>().Where(predicate));
            Save();
        }

        public virtual void Edit(TModel entity, Expression<Func<TEntity, object>> keyProperty)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TModel, TEntity>());
            var mapper = new Mapper(config);
            var item = mapper.Map<TModel, TEntity>(entity);
            try
            {
                var key = keyProperty.Compile().Invoke(item);
                var existingItem = _db.Set<TEntity>().Local.FirstOrDefault(e => keyProperty.Compile().Invoke(e).Equals(key));

                if (existingItem != null)
                {
                    _db.Entry(existingItem).State = EntityState.Detached;
                }

                _db.Entry(item).State = EntityState.Modified;

            }catch(Exception ex)
            {
                throw;
            }

            Save();
        }

        public virtual void Save()
        {
            _db.SaveChanges();
        }

        public IQueryable<TEntity> List(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includeProperties = null,
        int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();

            if (includeProperties != null)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (page != null && pageSize != null)
                query = query
                    .Skip(page.Value)
                    .Take(pageSize.Value);

            return query;
        }

        public IQueryable<TEntity> List(Expression<Func<TEntity, bool>> filter = null, string orderBy = null, string ascendingDescending = "ASC",
            List<Expression<Func<TEntity, object>>> includeProperties = null,
       int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();

            if (includeProperties != null)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (page != null && pageSize != null)
                query = query
                    .OrderBy(orderBy ?? "Id", ascendingDescending == "ASC")
                    .Skip(page.Value)
                    .Take(pageSize.Value);

            return query;
        }

        public Tuple<IQueryable<TEntity>, int> ListWithPaging(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includeProperties = null,
        int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();

            if (includeProperties != null)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            var count = query.Count();
            if (page != null && pageSize != null)
                query = query
                    .Skip(page.Value)
                    .Take(pageSize.Value);

            return new Tuple<IQueryable<TEntity>, int>(query, count);
        }

        public Tuple<IQueryable<TEntity>, int> ListWithPaging(Expression<Func<TEntity, bool>> filter = null, string orderBy = null, string ascendingDescending = "ASC",
           List<Expression<Func<TEntity, object>>> includeProperties = null,
      int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();

            if (includeProperties != null)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            var count = query.Count();

            if (page != null && pageSize != null)
                query = query
                    .OrderBy(orderBy ?? "Id", ascendingDescending == "ASC")
                    .Skip(page.Value)
                    .Take(pageSize.Value);

            return new Tuple<IQueryable<TEntity>, int>(query, count);
        }

        public IQueryable<TModel> ToDtoListPaging(List<TModel> list, string orderBy = null, string ascendingDescending = "ASC", int? page = null, int? pageSize = null)
        {
            IQueryable<TModel> query = list.AsQueryable();

            if (page != null && pageSize != null)
                query = query
                    .OrderBy(orderBy ?? "Id", ascendingDescending == "ASC")
                    .Skip(page.Value)
                    .Take(pageSize.Value);

            return query;
        }

        public virtual IEnumerable<TDto> MapToModelList<TEntity, TDto>(IEnumerable<TEntity> entity)
            where TEntity : class
            where TDto : class
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TEntity, TDto>());
            var mapper = new Mapper(config);
            return mapper.Map<IEnumerable<TEntity>, IEnumerable<TDto>>(entity);
        }

        public virtual IEnumerable<TEntity> MapToEntityList<TDto, TEntity>(IEnumerable<TDto> dto)
            where TDto : class
            where TEntity : class
        {

            var config = new MapperConfiguration(cfg => cfg.CreateMap<TDto, TEntity>());
            var mapper = new Mapper(config);
            return mapper.Map<IEnumerable<TDto>, IEnumerable<TEntity>>(dto);
        }
    }
}