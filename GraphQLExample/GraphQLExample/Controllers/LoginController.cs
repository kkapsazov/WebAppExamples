using System.Linq;
using GraphQLExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GraphQLExample.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly CoreDbContext dbContext;
        private readonly IConfiguration configuration;

        public LoginController(CoreDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromForm] UserFrom userForm)
        {
            User user = this.dbContext.Users
                .FirstOrDefault(x => x.Username.ToLower() == userForm.Username.ToLower());

            if (user != null)
            {
                string hash = Utils.HashPassword(userForm.Password, user.Salt);
                if (hash == user.Password)
                {
                    return this.Ok(Utils.GenerateJwtToken(user, this.configuration["Jwt:Key"], this.configuration["Jwt:Issuer"]));
                }

                return this.Unauthorized("Wrong user or password");
            }

            return this.Unauthorized("User id not found");
        }

        [HttpGet]
        //[AllowAnonymous]
        [Authorize]
        public IActionResult Hash(string pass)
        {
            string salt = Utils.GenerateSalt();
            string hashPass = Utils.HashPassword(pass, salt);
            return this.Ok(new
            {
                hashPass,
                salt
            });
        }

        public class UserFrom
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
