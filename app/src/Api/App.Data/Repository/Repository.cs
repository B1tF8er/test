using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using App.Core.Repository;

namespace App.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DataModel.Context dataContext;
        private readonly IDbSet<TEntity> dbSet;

        protected IDbFactory DbFactory { get; private set; }

        protected DataModel.Context DbContext
        {
            get { return dataContext ?? (dataContext = DbFactory.Init()); }
        }

        public Repository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            dbSet = DbContext.Set<TEntity>();
        }

        public TEntity Get(int id)
        {
            return dataContext.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return dataContext.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return dataContext.Set<TEntity>().Where(predicate);
        }

        public void Add(TEntity entity)
        {
            dataContext.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            dataContext.Set<TEntity>().Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            dataContext.Set<TEntity>().Remove(entity);
        }
    }
}