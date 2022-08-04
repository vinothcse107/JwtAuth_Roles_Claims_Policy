namespace Roles_Claims.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthManagementController : ControllerBase
{
      private readonly Context _context;
      private readonly ILogger<AuthManagementController> _logger;
      private readonly UserManager<IdentityUser> _userManager;
      private readonly RoleManager<IdentityRole> _roleManager;
      private readonly JwtConfig _jwtCongit;
      public AuthManagementController(Context context, ILogger<AuthManagementController> logger, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptionsMonitor<JwtConfig> optMoniter)
      {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtCongit = optMoniter.CurrentValue;
      }

      [HttpPost("Register")]
      public async Task<IActionResult> Register([FromBody] UserRegistrationDto user)
      {
            if (ModelState.IsValid)
            {
                  var exUser = await _userManager.FindByEmailAsync(user.Email);
                  if (exUser != null)
                  {

                        return BadRequest(new RegistrationResponse()
                        {
                              Errors = new List<string>() { "Invalid Payload" },
                              Success = false
                        });
                  }
                  var newUser = new IdentityUser()
                  {
                        Email = user.Email,
                        UserName = user.UserName
                  };
                  var create = await _userManager.CreateAsync(newUser, user.Password);
                  if (create.Succeeded)
                  {
                        var role = _userManager.AddToRoleAsync(newUser, "Member");
                        return Ok(new RegistrationResponse()
                        {
                              Success = true,
                              Token = await GenerateJwtTokenAsync(newUser)
                        });
                  }
                  else
                  {
                        return BadRequest(new RegistrationResponse()
                        {
                              Errors = create.Errors.Select(s => s.Description).ToList(),
                              Success = false
                        });
                  }
            }
            return BadRequest(new RegistrationResponse()
            {
                  Errors = new List<string>() { "Invalid Payload" },
                  Success = false
            });
      }

      [HttpPost("Login")]
      public async Task<IActionResult> login([FromBody] UserLoginRequest user)
      {
            if (ModelState.IsValid)
            {
                  var exUser = await _userManager.FindByEmailAsync(user.Email);
                  if (exUser == null)
                  {
                        return BadRequest(new RegistrationResponse()
                        {
                              Errors = new List<string>() { "Invalid login request" },
                              Success = false
                        });
                  }

                  var isCorrect = await _userManager.CheckPasswordAsync(exUser, user.Password);
                  if (!isCorrect)
                  {
                        return BadRequest(new RegistrationResponse()
                        {
                              Errors = new List<string>() { "Invalid login password" },
                              Success = false
                        });
                  }
                  var jwtToken = await GenerateJwtTokenAsync(exUser);
                  return Ok(new RegistrationResponse()
                  {
                        Token = jwtToken,
                        Success = true
                  });
            }
            return BadRequest(new RegistrationResponse()
            {
                  Errors = new List<string>() { "Invalid Payload" },
                  Success = false
            });

      }


      private async Task<List<Claim>> GetAllValidClaims(IdentityUser user)
      {
            var _options = new IdentityOptions();

            var claims = new List<Claim>
            {
                  new Claim("Id" , user.Id),
                  new Claim(JwtRegisteredClaimNames.Email , user.Email),
                  new Claim(JwtRegisteredClaimNames.Sub , user.Email),
                  new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
            };

            // Getting claims Assigned to User
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Get user roles and add to claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                  var role = await _roleManager.FindByNameAsync(userRole);

                  if (role != null)
                  {
                        claims.Add(new Claim(ClaimTypes.Role, userRole));

                        var roleClaims = await _roleManager.GetClaimsAsync(role);
                        foreach (var roleClaim in roleClaims)
                        {
                              claims.Add(roleClaim);
                        }
                  }
            }
            return claims;

      }
      private async Task<string> GenerateJwtTokenAsync(IdentityUser user)
      {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtCongit.Secret);
            var claims = await GetAllValidClaims(user);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                  Subject = new ClaimsIdentity(claims),
                  Expires = DateTime.UtcNow.AddMinutes(5),
                  SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;

            // Tokenized Format
            // Console.WriteLine(token.ToString());
            // { "alg":"HS256","typ":"JWT"}.{ "Id":"fcac14cc-4440-4738-a4df-3fbc95312bba",
            // "email":"vino@gmail.com","sub":"vino@gmail.com","jti":"cb59276b-f88b-48f0-b5d2-641f04c11b42",
            // "nbf":1659554999,"exp":1659555029,"iat":1659554999}
      }
}
