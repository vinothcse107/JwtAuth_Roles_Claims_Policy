// Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// <---------------><----------------><------------------>


// Add services to the container.
// --> Startup.cs => ConfigureService method (Optional for user Services)

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.AddDbContext<Context>(o =>
            o.UseSqlServer(builder.Configuration.GetConnectionString("SQL")));

builder.Services.AddAuthentication(o =>
            {
                  o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                  o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                  var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);
                  jwt.SaveToken = true;
                  jwt.TokenValidationParameters = new TokenValidationParameters
                  {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        RequireExpirationTime = false
                  };
            });

builder.Services.AddDefaultIdentity<IdentityUser>
      (o => o.SignIn.RequireConfirmedAccount = true)
      .AddEntityFrameworkStores<Context>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// <---------------><----------------><------------------>

// --> Startup.cs => Configure Method
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
      app.UseSwagger();
      app.UseSwaggerUI();
}
app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
