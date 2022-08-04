
namespace Roles_Claims.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TodoController : Controller
{
      private readonly ILogger<TodoController> _logger;

      public TodoController(ILogger<TodoController> logger)
      {
            _logger = logger;
      }
      [HttpGet]
      public IActionResult Hello()
      {
            return Ok("HelloWorld");
      }
}
