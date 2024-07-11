using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Optovka.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserPostsService, UserPostsService>();

builder.Services.AddSingleton<InMemoryCache>();

var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!);
builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddSingleton<IAuthorizationHandler, PermissionsAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy =>
            policy.AddRequirements(new PermissionsRequirement(Permissions.UserAccess)));
    options.AddPolicy("Admin", policy =>
            policy.AddRequirements(new PermissionsRequirement(Permissions.AdminAccess)));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description =
        "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
        "Enter 'Bearer' [space] and then your token in the text input below." +
        "\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {{
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
            },
            Scheme = "jwt",
            Name = JwtBearerDefaults.AuthenticationScheme,
            In = ParameterLocation.Header,
        },
        new List<string>()
    }});
});

var app = builder.Build();

async Task CreateRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new Dictionary<string, List<string>>{ 
        { "Admin", new List<string>{Permissions.AdminAccess, Permissions.UserAccess } }, 
        { "User", new List<string>{Permissions.UserAccess } } 
    };
    foreach (var role in roles)
    {
        var roleExist = await roleManager.RoleExistsAsync(role.Key);
        IdentityRole? identityRole;
        if (!roleExist)
        {
            identityRole = new IdentityRole(role.Key);
            var roleResult = await roleManager.CreateAsync(identityRole);
        }
        else
        {
            identityRole = await roleManager.FindByNameAsync(role.Key);
        }
        foreach (var claim in role.Value)
        {
            var claimToAdd = new Claim("permission", claim);
            var existingClaim = await roleManager.GetClaimsAsync(identityRole!);
            if (!existingClaim.Any(c => c.Type == claimToAdd.Type && c.Value == claimToAdd.Value))
            {
                await roleManager.AddClaimAsync(identityRole!, claimToAdd);
            }
        }
    }
}

using var scope = app.Services.CreateScope();
await CreateRoles(scope.ServiceProvider);
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseMetricServer();

app.Run();