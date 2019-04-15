using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Wpf.Model;
using AppWpf.Model;

namespace AppWpf.Design
{
    public class DesignDataService : IDataService
    {
        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await Task.FromResult<IEnumerable<User>>(Enumerable.Empty<User>());

            return users;
        }

        public async Task<User> GetUser(int userId)
        {
            var user = await Task.FromResult<User>(new User());

            return user;
        }

        public async Task<User> CreateUser(User user)
        {
            var createdUser = await Task.FromResult<User>(new User());

            return createdUser;
        }

        public async Task UpdateUser(User user)
        {
            await Task.CompletedTask;
        }

        public async Task DeleteUser(int userId)
        {
            await Task.CompletedTask;
        }
    }
}