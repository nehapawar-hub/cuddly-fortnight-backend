using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;  
using System.Text;
using UserAPI.Data;
using UserAPI.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration =configuration;
        }

        [HttpPost("login")]

        public IActionResult Login(LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(x => x.email == request.Email);
            if(user == null)
            {
                return Unauthorized("Invalid email or password");

            }

            if(user.password != request.Password)
            {
                return Unauthorized("Invalid email or password");
            }

            var claims =new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.user_id.ToString()),
                new Claim(ClaimTypes.Name,user.firstname),  
                new Claim(ClaimTypes.NameIdentifier,user.lastname),
                new Claim(ClaimTypes.Email,user.email),
                new Claim(ClaimTypes.Role,user.password)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"])),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { 

                token = jwt             
            });

            }
        }
    }
