using Microsoft.AspNetCore.Mvc;

namespace NetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("Error")]
        public IActionResult Error()
        {
            throw new Exception("Something went wrong");
        }
    }
}
