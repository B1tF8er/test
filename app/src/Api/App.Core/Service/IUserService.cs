using System.Collections.Generic;
using App.Core.Domain;

namespace App.Core.Service
{
    public interface IUserService
    {
        User GetUser(int id);
        IEnumerable<User> GetUsers();
        void CreateUser(User User);
        void UpdateUser(User User);
        void DeleteUser(User User);
        void SaveUser();
    }
}
