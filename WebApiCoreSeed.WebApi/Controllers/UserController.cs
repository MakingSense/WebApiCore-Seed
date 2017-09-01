using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.Models;
using WebApiCoreSeed.Domain.Services.Interfaces;
using WebApiCoreSeed.WebApi.Controllers.Dtos;
using WebApiCoreSeed.WebApi.Filters;

namespace WebApiCoreSeed.WebApi.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAsync());
        }

        // GET api/users/0E95A953-D023-4425-ED35-08D4E34D4DB8
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST api/users
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody]UserDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var affectedRows = await _userService.CreateAsync(new User
            {
                Id = Guid.NewGuid(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                CreatedBy = "Test"
                // TODO: get createdBy from current user
            });

            return affectedRows == 0 ? NotFound() : NoContent() as IActionResult;
        }

        // PUT api/users/0E95A953-D023-4425-ED35-08D4E34D4DB8
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

        // DELETE api/users/0E95A953-D023-4425-ED35-08D4E34D4DB8
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var affectedRows = await _userService.DeleteByIdAsync(id);

            return affectedRows == 0 ? NotFound() : NoContent() as IActionResult;
        }
    }
}

