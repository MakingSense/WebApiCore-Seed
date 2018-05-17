using Microsoft.AspNetCore.Mvc;
using Seed.Api.Filters;
using Seed.Api.Models;
using Seed.Data.Models;
using Seed.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seed.Api.Controllers
{
    [Route("api/users")]
    [Produces("Application/json")]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets a list of users
        /// </summary>
        /// <response code="200">A list of users</response>
        /// <return>A list of users</return>
        [HttpGet]
        [ProducesResponseType(typeof(List<User>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAsync());
        }

        /// <summary>
        /// Gets a user based on his id
        /// </summary>
        /// <param name="id" cref="Guid">Guid of the user</param>
        /// <response code="200">The user that has the given id</response>
        /// <response code="404">User with the given id was not found</response>
        /// <return>A users</return>
        [HttpGet("{id}")]
        [ValidateModel]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user" cref="UserDto">User model</param>
        /// <response code="201">User created</response>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(User), 201)]
        public async Task<IActionResult> Create([FromBody]UserDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userCreated = await _userService.CreateAsync(new User
            {
                Id = Guid.NewGuid(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                CreatedBy = "Test"
                // TODO: get createdBy from current user
            });

            return CreatedAtAction("Get", new { id = userCreated.Id }, userCreated);
        }

        ///<summary>
        /// Updates an user given his id
        ///</summary>
        ///<param name="id" cref="Guid">Guid of the user</param>
        ///<param name="user" cref="UserDto">User model</param>
        ///<response code="204">User created</response>
        ///<response code="404">User not found / User could not be updated</response>
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Update(Guid id, [FromBody]UserDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var affectedRows = await _userService.UpdateAsync(new User
            {
                Id = id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName
                // TODO: get UpdatedBy from current user
            });

            return affectedRows == 0 ? NotFound() : NoContent() as IActionResult;
        }

        ///<summary>
        /// Deletes an user given his id
        ///</summary>
        ///<param name="id" cref="Guid">Guid of the user</param>
        ///<response code="204">User Deleted</response>
        ///<response code="404">User not found / User could not be deleted</response>
        [HttpDelete("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Delete(Guid id)
        {
            var affectedRows = await _userService.DeleteByIdAsync(id);

            return affectedRows == 0 ? NotFound() : NoContent() as IActionResult;
        }
    }
}

