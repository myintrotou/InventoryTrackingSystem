using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WarehouseDashboard.API.Data;
using WarehouseDashboard.API.Models;
using BCrypt.Net;

namespace WarehouseDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly WarehouseContext _context;
        private readonly IConfiguration _config;

        public AuthController(WarehouseContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegisterDto request)
        {
            try 
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var user = new User { Username = request.Username, PasswordHash = passwordHash };
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("REGISTRATION ERROR: " + ex.Message);
                if (ex.InnerException != null) Console.WriteLine("INNER ERROR: " + ex.InnerException.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto request)
        {
            try 
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return BadRequest("Wrong username or password.");
                }

                string token = CreateToken(user);
                return Ok(new AuthResponseDto { Token = token, Username = user.Username });
            }
            catch (Exception ex)
            {
                Console.WriteLine("LOGIN ERROR: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value ?? "very_secret_key_that_is_at_least_32_chars_long"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
