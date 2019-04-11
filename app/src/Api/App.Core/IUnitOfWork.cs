using System;
using App.Core.Repository;

namespace App.Core
{
    public interface IUnitOfWork
    {
        int Complete();
    }
}
