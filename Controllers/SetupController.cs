using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roles_Claims.Controllers
{
      [ApiController]
      [Route("api/[controller]")]
      public class SetupController : ControllerBase
      {
            private readonly Context _context;
            private readonly UserManager<IdentityUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly ILogger<SetupController> _logger;

            public SetupController(Context context,
                  UserManager<IdentityUser> userManager,
                  RoleManager<IdentityRole> roleManager,
                  ILogger<SetupController> logger)
            {
                  _context = context;
                  _userManager = userManager;
                  _roleManager = roleManager;
                  _logger = logger;
            }


            [HttpGet("GetRoles")]
            public async Task<IActionResult> GetAllRoles()
            {
                  var roles = await _roleManager.Roles.ToListAsync();
                  return Ok(roles);
            }

            [HttpGet("GetAllUsers")]
            public async Task<IActionResult> GetAllUsers()
            {
                  var users = await _userManager.Users.ToListAsync();
                  return Ok(users);
            }

            [HttpGet("GetUserRoles")]
            public async Task<IActionResult> GetUserRoles(string email)
            {
                  var userCheck = await _userManager.FindByEmailAsync(email);
                  if (userCheck != null)
                  {
                        var roles = await _userManager.GetRolesAsync(userCheck);
                        return Ok(roles);
                  }
                  return BadRequest(new { result = "User Doesn't Exists !" });
            }

            [HttpPost("RemoveRoleFromUser")]
            public async Task<IActionResult> RemoveUserfromRole(string email, string roleName)
            {

                  var userCheck = await _userManager.FindByEmailAsync(email);
                  if (userCheck != null)
                  {
                        var role = await _roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                              var AssignRole = await _userManager.RemoveFromRoleAsync(userCheck, role.Name);
                              return AssignRole.Succeeded ?
                                          Ok(new { result = $"User {email} Removed From {role.Name} Role Successfully " })
                                          : BadRequest(new { result = $"Something Error occuerd" });
                        }
                        return BadRequest(new { result = $"{role.Name} Role Doesn't Exists !" });
                  }
                  return BadRequest(new { result = "User Doesn't Exists !" });

            }


            [HttpPost("AddRoleToUser")]
            public async Task<IActionResult> AddRoleToUser(string email, string roleName)
            {
                  var userCheck = await _userManager.FindByEmailAsync(email);
                  if (userCheck != null)
                  {
                        var role = await _roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                              var AssignRole = await _userManager.AddToRoleAsync(userCheck, role.Name);
                              return AssignRole.Succeeded ?
                                          Ok(new { result = $"User Successfully Assigned to Role" })
                                          : BadRequest(new { result = $"Something Error occuerd" });
                        }
                        return BadRequest(new { result = "Role Doesn't Exists !" });
                  }
                  return BadRequest(new { result = "User Doesn't Exists !" });
            }

            [HttpPost("AddRole")]
            public async Task<IActionResult> AddRole(string RoleName)
            {
                  string x;
                  var checkExist = await _roleManager.RoleExistsAsync(RoleName);
                  if (!checkExist)
                  {
                        var result = await _roleManager.CreateAsync(new IdentityRole(RoleName));
                        if (result.Succeeded)
                        {
                              x = $"The Role {RoleName} has added Successfully";
                              _logger.LogInformation(x);
                              return Ok(new { result = $"{x}" });
                        }
                        else
                        {
                              x = $"The Role {RoleName} has not added, Something went wrong";
                              _logger.LogInformation(x);
                              return BadRequest(new { result = $"{x}" });
                        }
                  }
                  return BadRequest(new { Error = $"{RoleName} Role Already Exists" });
            }

            [HttpDelete("DeleteRole")]
            public async Task<IActionResult> DeleteRole(string RoleName)
            {
                  string y;
                  var checkExist = await _roleManager.FindByNameAsync(RoleName);
                  if (checkExist != null)
                  {
                        y = $"{RoleName} has deleted Successfully";
                        await _roleManager.DeleteAsync(checkExist);
                        _logger.LogInformation(y);
                        return Ok(new { result = y });
                  }
                  y = $"{RoleName} Dosen't Exists !";
                  _logger.LogInformation(y);
                  return BadRequest(new { result = y });
            }
      }
}