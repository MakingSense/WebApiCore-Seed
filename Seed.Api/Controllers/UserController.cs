using Microsoft.AspNetCore.Mvc;
using Seed.Api.Filters;
using Seed.Api.Models;
using Seed.Data.Models;
using Seed.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <response code="200">List of users</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAsync();
            var result = users.Select(u => new UserDto(u)).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Gets a user for the given id
        /// </summary>
        /// <param name="userId" cref="Guid">Guid of the user</param>
        /// <response code="200">User for the given id</response>
        /// <response code="404">User not found</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        public async Task<IActionResult> Get(Guid userId)
        {
            var user = await _userService.GetByIdAsync(userId);

            if (user == null) return NotFound();

            var result = new UserDto(user);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user" cref="InputUserDto">User model</param>
        /// <response code="201">User created</response>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(User), 201)]
        public async Task<IActionResult> Create([FromBody]InputUserDto user)
        {
            // TODO: Fix validation attribute, it's not working as expected.
            if (user == null) return BadRequest();

            var userToCreate = new User
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName
            };

            var result = await _userService.CreateAsync(userToCreate);

            return CreatedAtAction(nameof(Get), new { userId = result.Id }, new UserDto(result));
        }

        ///<summary>
        /// Updates an user given his id
        ///</summary>
        ///<param name="id" cref="Guid">Guid of the user</param>
        ///<param name="user" cref="InputUserDto">User model</param>
        ///<response code="204">User created</response>
        ///<response code="404">User not found / User could not be updated</response>
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Update(Guid id, [FromBody]InputUserDto user)
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

