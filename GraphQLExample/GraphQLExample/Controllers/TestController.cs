using Microsoft.AspNetCore.Mvc;

namespace GraphQLExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok();
        }
    }
}
