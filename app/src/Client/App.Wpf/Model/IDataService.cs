using App.Wpf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWpf.Model
{
    public interface IDataService
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int userId);
        Task<User> CreateUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(int userId);
    }
}
