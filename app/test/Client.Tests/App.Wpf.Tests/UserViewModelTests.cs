
using App.Wpf.Model;
using AppWpf.Model;
using AppWpf.ViewModel;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Windows;

namespace App.Wpf.Tests
{
    
    public class UserViewModelTests : IDisposable
    {
        List<User> _users;
        IDataService _dataService;

        public UserViewModelTests()
        {
            _users = SetupUsers();
            _dataService = SetupDataService();
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

        private IDataService SetupDataService()
        {
            var dataService = new Mock<IDataService>();

            //Setup mock behavior
            dataService.Setup(ds => ds.GetUsers()).Returns(Task.FromResult(_users as IEnumerable<User>));

            dataService.Setup(ds => ds.GetUser(It.Is<int>(id => id <= 0)))
                    .Throws(new System.Exception("Not Found"));

            dataService.Setup(ds => ds.GetUser(It.Is<int>(id => id > 0)))
                    .Returns(new Func<int, Task<User>>(id => Task.FromResult(_users.Find(u => u.Id.Equals(id)))));

            dataService.Setup(ds => ds.CreateUser(It.IsAny<User>()))
                    .Callback(new Action<User>(newUser =>
                    {
                        dynamic maxUserId = _users.Last().Id;
                        dynamic nextUserId = maxUserId + 1;
                        newUser.Id = nextUserId;
                        _users.Add(newUser);
                    }))
                    .Returns(Task.FromResult(_users.Last()));

            dataService.Setup(ds => ds.UpdateUser(It.IsAny<User>()))
                    .Callback(new Action<User>(x =>
                    {
                        var oldUser = _users.Find(u => u.Id == x.Id);
                        oldUser.Name = x.Name;
                        oldUser.Avatar = x.Avatar;
                        oldUser.Email = x.Email;
                    }))
                    .Returns(Task.CompletedTask);

            dataService.Setup(ds => ds.DeleteUser(It.IsAny<int>()))
                    .Callback(new Action<int>(id =>
                    {
                        var userToRemove = _users.Find(u => u.Id == id);

                        if (userToRemove != null)
                            _users.Remove(userToRemove);
                    }))
                    .Returns(Task.CompletedTask); ;

            // Return mock implementation
            return dataService.Object;
        }

        [Fact]
        public void RefreshUsersList_NoParameters_RefreshesUsersList()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);

            var users = userViewModel.Users;

            Assert.NotEmpty(users);
        }

        [Fact]
        public void CanSaveUser_InvalidUserEmail_ReturnsFalse()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);
            var validName = "ValidName";
            var validAvatar = new byte[1];
            var invalidEmail = "InvalidEmail@";


            userViewModel.TempUser = new User();
            userViewModel.TempUser.Name = validName;
            userViewModel.TempUser.Avatar = validAvatar;
            userViewModel.TempUser.Email = invalidEmail;

            var result = userViewModel.CanSaveUser();

            Assert.False(result);
        }

        [Fact]
        public void CanSaveUser_InvalidMinimumAge_ReturnsFalse()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);
            var validName = "ValidName";
            var validAvatar = new byte[1];
            var validEmail = "ValidNameEmail@email.com";
            var invalidMinimumAge = 0;


            userViewModel.TempUser = new User();
            userViewModel.TempUser.Name = validName;
            userViewModel.TempUser.Avatar = validAvatar;
            userViewModel.TempUser.Email = validEmail;
            userViewModel.TempUser.Age = invalidMinimumAge;

            var result = userViewModel.CanSaveUser();

            Assert.False(result);
        }

        [Fact]
        public void CanSaveUser_InvalidMaximumAge_ReturnsFalse()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);
            var validName = "ValidName";
            var validAvatar = new byte[1];
            var validEmail = "ValidNameEmail@email.com";
            var invalidMaximumAge = 131;


            userViewModel.TempUser = new User();
            userViewModel.TempUser.Name = validName;
            userViewModel.TempUser.Avatar = validAvatar;
            userViewModel.TempUser.Email = validEmail;
            userViewModel.TempUser.Age = invalidMaximumAge;

            var result = userViewModel.CanSaveUser();

            Assert.False(result);
        }

        [Fact]
        public void CanSaveUser_ValidUserPropierties_ReturnsTrue()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);
            var validName = "ValidName";
            var validAvatar = new byte[1];
            var validEmail = "ValidEmail@mail.com";
            var validAge = 1;

            userViewModel.TempUser = new User();
            userViewModel.TempUser.Name = validName;
            userViewModel.TempUser.Avatar = validAvatar;
            userViewModel.TempUser.Email = validEmail;
            userViewModel.TempUser.Age = validAge;
            var result = userViewModel.CanSaveUser();

            Assert.True(result);
        }

        [Fact]
        public void CreateUserHandler_NoParameters_MakesUserPopupVisible()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);

            userViewModel.CreateUserHandler();

            Assert.Equal(Visibility.Visible, userViewModel.IsUserPopupOpen);
        }

        [Fact]
        public void EditUserHandler_UserIdToEdit_GetTheUserToEdit()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);
            var UserIdToEdit = 1;

            userViewModel.EditUserHandler(UserIdToEdit);

            Assert.Equal(userViewModel.TempUser.Id, UserIdToEdit);
        }

        [Fact]
        public void CloseUserPopupHandler_NoParameters_MakesUserPopupHidden()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);

            userViewModel.TempUser = new User();
            userViewModel.CloseUserPopupHandler();

            Assert.Equal(Visibility.Hidden, userViewModel.IsUserPopupOpen);
        }

        [Fact]
        public void SaveUserHandler_NewUser_CreatesNewUser()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);

            User userToCreate = new User();
            userToCreate.Name = "NewUser";
            userToCreate.Avatar = new byte[1];
            userToCreate.Email = "NewUser@email.com";

            userViewModel.TempUser = userToCreate;
            userViewModel.SaveUserHandler();
            var lastCreatedUser = userViewModel.Users.Last();

            Assert.Equal(lastCreatedUser.Email, userToCreate.Email);
        }

        [Fact]
        public void SaveUserHandler_ExistingUser_UpdatesExistingUser()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);
            int existingUserId = 1;
            User userToUpdate = new User();

            userToUpdate.Id = existingUserId;
            userToUpdate.Name = "UserNameUpdated";
            userToUpdate.Avatar = new byte[3];
            userToUpdate.Email = "UserEmailUpdated@email.com";

            userViewModel.TempUser = userToUpdate;
            userViewModel.SaveUserHandler();
            var updatedUser = userViewModel.Users.First();

            Assert.Equal(updatedUser.Email, userToUpdate.Email);
        }

        [Fact]
        public void DeleteUserHandler_ExistingUser_DeletesExistingUser()
        {
            UserViewModel userViewModel = new UserViewModel(_dataService);
            int existingUserId = 2;

            userViewModel.DeleteUserHandler(existingUserId);
            var deletedUser = userViewModel.Users.Where(u => u.Id == existingUserId).FirstOrDefault();

            Assert.Null(deletedUser);
        }

        public void Dispose()
        {
            _dataService = null;
        }
    }
}
