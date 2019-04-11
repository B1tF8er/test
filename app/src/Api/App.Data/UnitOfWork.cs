using App.Core;
using App.Core.Repository;

namespace App.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory dbFactory;
        private DataModel.Context dbContext;

        public UnitOfWork(IDbFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public DataModel.Context DbContext
        {
            get { return dbContext ?? (dbContext = dbFactory.Init()); }
        }

        public int Complete()
        {
            return dbContext.SaveChanges();
        }
    }
}
