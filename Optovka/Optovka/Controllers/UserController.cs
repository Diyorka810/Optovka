using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Optovka.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<ApplicationUser> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    [HttpGet("getById")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Get([FromQuery] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var userInfo = new UserInfoDto() { 
                Username = user.UserName!, 
                PhoneNumber = user.PhoneNumber!,
                Email = user.Email!,
                BirthDate = user.BirthDate! 
            };

            return Ok(userInfo);
        }
        return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", Message = "User not found" });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
    {
        var userExists = await _userManager.FindByNameAsync(registerModel.UserName);
        if (userExists != null)
            return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", Message = "User already exists!" });

        var user = new ApplicationUser()
        {
            Email = registerModel.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerModel.UserName,
            PhoneNumber = registerModel.PhoneNumber,
            BirthDate = registerModel.BirthDate
        };

        var (isValid, errorsList) = user.IsValid();
        if (!isValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", Message = errorsList.ToString()});
        }


        var result = await _userManager.CreateAsync(user, registerModel.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", Message = result.Errors });

        await _userManager.AddToRoleAsync(user, "User");
        return StatusCode(StatusCodes.Status201Created, new { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {

            var authClaims = new List<Claim>
            {
                //вынести в константы
                new Claim("userId", user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            foreach (var roleName in rolesList)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var existingClaims = await _roleManager.GetClaimsAsync(role);
                authClaims.AddRange(existingClaims);
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }
}