using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using App.Core.Service;
using App.Core.Domain;
using System.Web.Http.Description;
using System.Data.Entity.Infrastructure;

namespace App.WebApi.Controllers
{
    public class UsersController : ApiController
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private bool UserExists(int id)
        {
            return _userService.GetUser(id) != null;
        }

        // GET: api/Users
        public IEnumerable<User> Get()
        {
            return _userService.GetUsers();
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult Get(int id)
        {
            User user = _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult Post(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _userService.CreateUser(user);

            try
            {
                _userService.SaveUser();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Put(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            if (!UserExists(id))
            {
                return NotFound();
            }

            _userService.UpdateUser(user);

            try
            {
                _userService.SaveUser();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult Delete(int id)
        {
            User user = _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.DeleteUser(user);

            try
            {
                _userService.SaveUser();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(user);
        }
    }
}
