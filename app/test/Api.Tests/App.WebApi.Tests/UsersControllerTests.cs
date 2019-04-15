using App.Core;
using App.Core.Domain;
using App.Core.Repository;
using App.Core.Service;
using App.WebApi.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Xunit;


namespace App.WebApi.Tests
{
    public class UsersControllerTests : IDisposable
    {
        IUserService _userService;
        IUserRepository _userRepository;
        IUnitOfWork _unitOfWork;
        List<User> _users;

        public UsersControllerTests()
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
        public void Get_NoFilters_ReturnsAllUsers()
        {
            var usersController = new UsersController(_userService);

            var returnedUsers = usersController.Get();

            Assert.Equal(_users, returnedUsers);
        }

        [Fact]
        public void Get_LastUserId_ReturnsLastUser()
        {
            var usersController = new UsersController(_userService);
            var LastUserId = 3;

            var returnedUser = usersController.Get(LastUserId) as OkNegotiatedContentResult<User>;

            Assert.NotNull(returnedUser);
            Assert.Equal(_users.Last(), returnedUser.Content);
        }

        [Fact]
        public void Put_InvalidDifferentUserId_ReturnsBadRequestResult()
        {
            var userId = 1;
            var invalidDifferentUserId = -1;
            var usersController = new UsersController(_userService)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"http://localhost/api/users/{userId}")
                }
            };
            var invalidUser = new User()
            {
                Id = invalidDifferentUserId,
                Name = "Invalid User",
                Avatar = new byte[1],
                Email = "invalidUser@email.com"
            };

            var badresult = usersController.Put(userId, invalidUser);

            Assert.IsType<BadRequestResult>(badresult);
        }

        [Fact]
        public void Put_ValidFirstUserId_UpdatesFirstUser()
        {
            var validUserId = 1;
            var usersController = new UsersController(_userService)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"http://localhost/api/users/{validUserId}")
                }
            };
            var newUserName = "New User Name";
            var validUser = new User()
            {
                Id  = validUserId,
                Name = newUserName,
                Avatar = new byte[1],
                Email = "newUser@email.com"
            };

            IHttpActionResult updateResult = usersController.Put(validUserId, validUser);

            Assert.IsType<StatusCodeResult>(updateResult);
            StatusCodeResult statusCodeResult = updateResult as StatusCodeResult;
            Assert.Equal(HttpStatusCode.NoContent, statusCodeResult.StatusCode);
            Assert.Equal(newUserName, _users.First().Name);
        }

        [Fact]
        public void Post_InvalidNewUserFields_NotPostNewUser()
        {
            byte[] invalidAvatarValue = null;
            var modelErrorCounter = 1;

            var newUser = new User()
            {
                Name = "New User Name",
                Avatar = invalidAvatarValue,
                Email = "newUser@email.com"
            };

            var usersController = new UsersController(_userService)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://localhost/api/users")
                }
            };

            usersController.Configuration.MapHttpAttributeRoutes();
            usersController.Configuration.EnsureInitialized();
            usersController.RequestContext.RouteData = new HttpRouteData(
                new HttpRoute(),
                new HttpRouteValueDictionary { { "Controller", "Users" } }
            );

            usersController.ModelState.AddModelError("Avatar", "Avatar is required field");

            var result = usersController.Post(newUser) as InvalidModelStateResult;

            Assert.Equal(modelErrorCounter, result.ModelState.Count);
            Assert.False(result.ModelState.IsValid);
        }

        [Fact]
        public void Post_ValidNewUserFields_PostNewUser()
        {
            var newUser = new User()
            {
                Name = "New User Name",
                Avatar = new byte[1],
                Email = "newUser@email.com"
            };

            var usersController = new UsersController(_userService) 
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://localhost/api/users")
                }
            };

            usersController.Configuration.MapHttpAttributeRoutes();
            usersController.Configuration.EnsureInitialized();
            usersController.RequestContext.RouteData = new HttpRouteData(
                new HttpRoute(), 
                new HttpRouteValueDictionary { { "Controller", "Users" } }
            );

            var expectedRoutName = "DefaultApi";

            var result = usersController.Post(newUser) as CreatedAtRouteNegotiatedContentResult<User>;
            var postedUser = result.Content;
            var routeIdValue = result.RouteValues["id"];
            var lastUserId = _users.Last().Id;

            Assert.Equal(expectedRoutName, result.RouteName);
            Assert.Equal(routeIdValue, postedUser.Id);
            Assert.Equal(postedUser.Id, lastUserId);
        }

        [Fact]
        public void Delete_InvalidUserId_ReturnsNotFoundResult()
        {
            var invalidUserId = -1;

            var usersController = new UsersController(_userService)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"http://localhost/api/users/{invalidUserId}")
                }
            };

            var result = usersController.Delete(invalidUserId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ValidUserId_DeletesLastUser()
        {
            var lastUserId = 3;
            var usersController = new UsersController(_userService)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"http://localhost/api/users/{lastUserId}")
                }
            };

            var result = usersController.Delete(lastUserId);

            Assert.IsType<OkNegotiatedContentResult<User>>(result);
            Assert.Null(_users.Find(u => u.Id == lastUserId));
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
