using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Data;
using VidizmoBackend.Services;
using VidizmoBackend.Repositories;
using VidizmoBackend.Helpers;
using Microsoft.AspNetCore.Identity;
using VidizmoBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http.Features;
using VidizmoBackend.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services and repositories
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IOrgRepository, OrgRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AzureBlobService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddSingleton<JwtTokenGenerator>(); // Assuming stateless JWT helper
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();

builder.Services.AddSingleton(sp => {
  var conn = sp.GetRequiredService<IConfiguration>()["AzureServiceBus:ConnectionString"];
  return new ServiceBusClient(conn);
});
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddHostedService<AuditLogProcessor>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
    });
builder.Services.AddAuthorization();
// Add controllers
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 500; // 500MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 1024 * 1024 * 500; // 500MB
});

var app = builder.Build();
app.UseCors();
app.UseHttpsRedirection();
app.UseMiddleware<TokenMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditLoggingMiddleware>();
app.MapControllers();

app.Run();
