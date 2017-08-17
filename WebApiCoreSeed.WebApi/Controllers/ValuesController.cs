using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApiCoreSeed.Data.EF;
using WebApiCoreSeed.Data.Models;
using WebApiCoreSeed.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace WebApiCoreSeed.WebApi.Controllers
{

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {

            var createdUser =
               new User
               {

                   FirstName = "John",
                   LastName = "Doe",
                   Email = "JohnDoe@makinsense.com",
                   UserName = "JohnDoe"
               };

            return Ok(createdUser);
            //return Ok(new string[] { "value1", "value2" });
        }

        private async Task<List<User>> GetAsyn()
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            List<User> users = new List<User>();
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);
                users = await userService.GetAsync();
            }
            return users;
        }

        // GET api/values/0E95A953-D023-4425-ED35-08D4E34D4DB8
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            User retrievedUser = null;
            retrievedUser = await GetAsync(id);

            if (retrievedUser == null)
            {
                return NotFound();
            }



            return Ok(retrievedUser);



        }





        private async Task<User> GetAsync(Guid id)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            User retrievedUser = null;

            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);
                retrievedUser = await userService.GetByIdAsync(id);
            }

            return retrievedUser;
        }



        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User user)
        {

            string result = string.Empty;

            if (user == null)
            {
                return BadRequest();
            }

            user.Id = Guid.NewGuid();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            result = await PostAsync(user);

            //return CreatedAtRoute("Get", new
            //{ id = id }, user);
            return StatusCode(201, result);



        }


        private async Task<string> PostAsync(User user)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            string result = null;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);
                result = await userService.CreateAsync(user);
            }
            return result;
        }



        // PUT api/values/0E95A953-D023-4425-ED35-08D4E34D4DB8
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]User user)
        {
            string result = string.Empty;
            if (user == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Id = id;
            result = await PutAsync(user);

            return NoContent();
        }


        private async Task<string> PutAsync(User user)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            string result = null;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);
                result = await userService.UpdateAsync(user);
            }
            return result;
        }


        // DELETE api/values/0E95A953-D023-4425-ED35-08D4E34D4DB8
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            string result = string.Empty;
            var myTask = DeleteAsync(id);
            result = myTask.Result;



            return NoContent();
        }


        private async Task<string> DeleteAsync(Guid id)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            string result = string.Empty;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))

            {
                var userService = new UserService(context);
                result = await userService.DeleteByIdAsync(id);
            }
            return result;
        }
    }
}
