namespace Roles_Claims.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthManagementController : ControllerBase
{
      private readonly UserManager<IdentityUser> _userManager;
      private readonly JwtConfig _jwtCongit;
      public AuthManagementController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optMoniter)
      {
            _userManager = userManager;
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
                        return Ok(new RegistrationResponse()
                        {
                              Success = true,
                              Token = GenerateJwtToken(newUser)
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
                  var jwtToken = GenerateJwtToken(exUser);
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


      private string GenerateJwtToken(IdentityUser user)
      {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtCongit.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                  Subject = new ClaimsIdentity(new[]{
                        new Claim("Id" , user.Id),
                        new Claim(JwtRegisteredClaimNames.Email , user.Email),
                        new Claim(JwtRegisteredClaimNames.Sub , user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
                  }),
                  Expires = DateTime.UtcNow.AddSeconds(20),
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
