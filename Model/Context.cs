using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Roles_Claims.Model;
public class Context : IdentityDbContext
{
      public Context(DbContextOptions<Context> options)
          : base(options)
      { }


}
