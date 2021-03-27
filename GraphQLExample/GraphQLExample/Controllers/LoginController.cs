using System.Linq;
using GraphQLExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraphQLExample.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly CoreDbContext dbContext;

        public LoginController(CoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] UserFrom userForm)
        {
            User user = this.dbContext.Users
                .FirstOrDefault(x => x.Username.ToLower() == userForm.Username.ToLower());

            if (user != null)
            {
                string hash = Utils.HashPassword(userForm.Password, user.Salt);
                if (hash == user.Password)
                {
                    return this.Ok(new UserDto
                    {
                        ID = user.Id,
                        Username = user.Username
                    });
                }

                return this.Unauthorized("Wrong user or password");
            }

            return this.Unauthorized("User id not found");
        }

        public class UserFrom
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class UserDto
        {
            public int ID { get; set; }
            public string Username { get; set; }
        }
    }
}
