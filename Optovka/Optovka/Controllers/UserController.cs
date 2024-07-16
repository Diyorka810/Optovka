using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Optovka.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Optovka
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private const string ClaimName = "userId";

        public UserController(UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager)
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
                var userInfo = new UserInfoDto()
                {
                    UserName = user.UserName!,
                    PhoneNumber = user.PhoneNumber!,
                    Email = user.Email!,
                    BirthDate = user.BirthDate!
                };

                return Ok(userInfo);
            }
            return StatusCode(
                StatusCodes.Status400BadRequest, 
                new ApiResponseDto { Status = "Error", Message = "User not found" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerModel)
        {
            var userExists = await _userManager.FindByNameAsync(registerModel.UserName);
            if (userExists != null)
                return StatusCode(
                    StatusCodes.Status400BadRequest, 
                    new ApiResponseDto { Status = "Error", Message = "User already exists!" });

            var user = new ApplicationUser()
            {
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.UserName,
                PhoneNumber = registerModel.PhoneNumber,
                BirthDate = registerModel.BirthDate,
                CardNumber = registerModel.CardNumber
            };

            var (isValid, errorsList) = user.IsValid();
            if (!isValid)
            {
                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new ApiResponseDto { Status = "Error", Message = errorsList.ToString() });
            }

            var createResult = await _userManager.CreateAsync(user, registerModel.Password);
            if (!createResult.Succeeded)
                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new ApiResponseDto
                    {
                        Status = "Error",
                        Message = string.Join("\n", createResult.Errors.Select(s => s.Description).ToList())
                    });

            await _userManager.AddToRoleAsync(user, "User");

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponseDto { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return Unauthorized();

            var authClaims = new List<Claim>
            {
                new Claim(ClaimName, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            foreach (var roleName in rolesList)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var existingClaims = await _roleManager.GetClaimsAsync(role!);
                authClaims.AddRange(existingClaims);
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

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
    }
}