
namespace Roles_Claims.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager , Member")]
public class TodoController : Controller
{
      private readonly ILogger<TodoController> _logger;

      public TodoController(ILogger<TodoController> logger)
      {
            _logger = logger;
      }
      [HttpGet("Hello_Auth")]
      public IActionResult Hello()
      {
            return Ok("HelloWorld");
      }

      [HttpGet("World_Policy")]
      [Authorize(Policy = "AdminAccessPolicy")]
      public IActionResult World()
      {
            return Ok("World");
      }
}
