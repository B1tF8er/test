using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data
{
    public interface IDbFactory : IDisposable
    {
        DataModel.Context Init();
    }
}
