
namespace Roles_Claims.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ClaimsSetupController : ControllerBase
{
      private readonly Context _context;
      private readonly ILogger<ClaimsSetupController> _logger;
      private readonly UserManager<IdentityUser> _userManager;
      private readonly RoleManager<IdentityRole> _roleManager;
      private readonly JwtConfig _jwtCongit;
      public ClaimsSetupController(Context context, ILogger<ClaimsSetupController> logger, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptionsMonitor<JwtConfig> optMoniter)
      {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtCongit = optMoniter.CurrentValue;
      }

      [HttpGet("GetAllClaims")]
      public async Task<IActionResult> GetAllClaims(string email)
      {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                  _logger.LogInformation($"This user wit the {email} does not exist");
                  return BadRequest(new
                  {
                        error = "user does not exist"
                  });
            }
            var userClaims = await _userManager.GetClaimsAsync(user);
            return Ok(userClaims);
      }

      [HttpPost("AddClaimsToUser")]
      public async Task<IActionResult> AddClaimsToUser(string email, string claimName, string claimValue)
      {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                  _logger.LogInformation($"This user wit the {email} does not exist");
                  return BadRequest(new
                  {
                        error = "user does not exist"
                  });
            }

            var userClaim = new Claim(claimName, claimValue);
            var result = await _userManager.AddClaimAsync(user, userClaim);
            return result.Succeeded ? Ok(new { result = $"User {user.Email} has a claim {claimName} added" })
                        : BadRequest(new { error = $"Unable to add Claims {claimName} to User {user.Email}" });
      }

      [HttpDelete("RemoveClaimFromUser")]
      public async Task<IActionResult> RemoveClaimFromUser(string email, string claimName)
      {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                  _logger.LogInformation($"This user wit the {email} does not exist");
                  return BadRequest(new
                  {
                        error = "user does not exist"
                  });
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var s = claims.Where(w => w.Type.Equals(claimName)).First();
            var result = await _userManager.RemoveClaimAsync(user, s);

            return result.Succeeded ? Ok($"{claimName} Claim Removed") : BadRequest($"Error Occuerd");
      }

}
