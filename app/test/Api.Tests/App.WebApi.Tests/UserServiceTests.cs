using App.Core;
using App.Core.Domain;
using App.Core.Repository;
using App.Core.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace App.WebApi.Tests
{
    public class UserServiceTests : IDisposable
    {
        IUserService _userService;
        IUserRepository _userRepository;
        IUnitOfWork _unitOfWork;
        List<User> _users;

        public UserServiceTests()
        {
            Setup();
        }

        private void Setup()
        {
            _users = SetupUsers();
            _userRepository = SetupUserRepository();
            _unitOfWork = new Mock<IUnitOfWork>().Object;
            _userService = new UserService(_userRepository, _unitOfWork);
        }

        private List<User> SetupUsers()
        {
            List<User> users = new List<User>() {
                new User(){
                    Id = 1,
                    Name = "User1",
                    Avatar = new byte[1],
                    Email = "user1@email.com"
                },
                new User(){
                    Id = 2,
                    Name = "User2",
                    Avatar = new byte[1],
                    Email = "user2@email.com"
                },
                new User(){
                    Id = 3,
                    Name = "User3",
                    Avatar = new byte[1],
                    Email = "user3@email.com"
                },
            };

            return users;
        }

        private IUserRepository SetupUserRepository()
        {
            var repository = new Mock<IUserRepository>();

            //Setup mock behavior
            repository.Setup(r => r.Get(It.IsAny<int>()))
                    .Returns(new Func<int, User>(id => _users.Find(u => u.Id.Equals(id))));

            repository.Setup(r => r.GetAll()).Returns(_users);

            repository.Setup(r => r.Find(It.IsAny<Expression<Func<User, bool>>>()))
                    .Returns(new Func<Expression<Func<User, bool>>, IEnumerable<User>>(f => _users.FindAll(new Predicate<User>(f.Compile()))));

            repository.Setup(r => r.Add(It.IsAny<User>()))
                    .Callback(new Action<User>(newUser =>
                    {
                        dynamic maxUserId = _users.Last().Id;
                        dynamic nextUserId = maxUserId + 1;
                        newUser.Id = nextUserId;
                        _users.Add(newUser);
                    }));

            repository.Setup(r => r.Update(It.IsAny<User>()))
                    .Callback(new Action<User>(x =>
                    {
                        var oldUser = _users.Find(u => u.Id == x.Id);
                        oldUser.Name = x.Name;
                        oldUser.Avatar = x.Avatar;
                        oldUser.Email = x.Email;
                    }));

            repository.Setup(r => r.Remove(It.IsAny<User>()))
                    .Callback(new Action<User>(x =>
                    {
                        var userToRemove = _users.Find(u => u.Id == x.Id);

                        if (userToRemove != null)
                            _users.Remove(userToRemove);
                    }));

            // Return mock implementation
            return repository.Object;
        }

        [Fact]
        public void GetUser_FirstUserId_ReturnsFirstUser()
        {
            int firstUserId = 1;
            var firstUser = _users.First();

            var user = _userService.GetUser(firstUserId);

            Assert.Equal(user, firstUser);
        }

        [Fact]
        public void GetUsers_NoFilters_ReturnsAllUsers()
        {
            var allUsers = _users;

            var returnedUsers = _userService.GetUsers();

            Assert.Equal(returnedUsers, allUsers);
        }

        [Fact]
        public void CreateUser_NewUser_AddsNewUser()
        {
            var newUser = new User() {Name = "NewUser", Avatar = new byte[1], Email = "newUser@Email.com"};

            _userService.CreateUser(newUser);

            var lastCreatedUser = _users.Last();

            Assert.Equal(newUser, lastCreatedUser);
        }

        [Fact]
        public void UpdateUser_UserToUpdate_UpdatesUser()
        {
            var userToUpdate = _users.First();
            var newUserName = "UpdatedName";
            var newUserAvatar = new byte[]{ 0x20 };
            var newUserEmail = "updatedEmail@Email.com";

            userToUpdate.Name = newUserName;
            userToUpdate.Avatar = newUserAvatar;
            userToUpdate.Email = newUserEmail;

            _userService.UpdateUser(userToUpdate);

            var updatedUser = _users.First();

            Assert.Equal(userToUpdate, updatedUser);
        }

        [Fact]
        public void DeleteUser_UserToDelete_DeletesUser()
        {
            var userToDelete = _users.Last();

            _userService.DeleteUser(userToDelete);

            Assert.DoesNotContain(userToDelete, _users);
        }

        public void Dispose()
        {
            _users = null;
            _userRepository = null;
            _unitOfWork = null;
            _userService = null;
        }
    }
}