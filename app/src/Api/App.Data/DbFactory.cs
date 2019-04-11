using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data
{
    public class DbFactory : Disposable, IDbFactory
    {
        DataModel.Context dbContext;

        public DataModel.Context Init()
        {
            return dbContext ?? (dbContext = new DataModel.Context());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
