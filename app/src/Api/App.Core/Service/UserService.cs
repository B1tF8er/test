using System.Collections.Generic;
using System.Linq;
using App.Core.Repository;
using App.Core.Domain;
using System.Reflection;

namespace App.Core.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository usersRepository;
        private readonly IUnitOfWork unitOfWork;

        public UserService(IUserRepository UsersRepository, IUnitOfWork unitOfWork)
        {
            this.usersRepository = UsersRepository;
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<User> GetUsers()
        {
            return usersRepository.GetAll();
        }

        public User GetUser(int id)
        {
            return usersRepository.Get(id);
        }

        public void CreateUser(User User)
        {
            usersRepository.Add(User);
        }

        public void UpdateUser(User User)
        {
            var targetUser = usersRepository.Get(User.Id);

            foreach (PropertyInfo property in typeof(User).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(targetUser, property.GetValue(User, null), null);
                }
            }

            usersRepository.Update(targetUser);
        }

        public void DeleteUser(User User)
        {
            usersRepository.Remove(User);
        }

        public void SaveUser()
        {
            unitOfWork.Complete();
        }

    }
}