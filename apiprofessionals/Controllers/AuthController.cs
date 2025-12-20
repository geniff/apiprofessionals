using apiprofessionals.RegisterDto;
using apiprofessionals.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace apiprofessionals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly VendingDbContext _db;
        public AuthController(VendingDbContext db) { _db = db; }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto.RegisterDto registerDto)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Email == registerDto.Email);
            if (existingUser != null) {
                return BadRequest("Пользователь уже существует");
            }

            //Создаю нового опльзователя
            var user = new UserModel();
            user.FullName = registerDto.FullName;
            user.Email = registerDto.Email;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            user.Role = "admin";

            _db.Users.Add(user);
            _db.SaveChanges();
            return Ok("Пользователь зарегистрирован");
        }
            [HttpPost("login")]
            public IActionResult Login([FromBody] LoginDto dto)
            {
                var user = _db.Users.FirstOrDefault(x => x.Email == dto.Email);
                if (user == null)
                    return Unauthorized("Неверный логин или пароль");

                bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                if (!valid)
                    return Unauthorized("Неверный логин или пароль");

                var token = GenerateJwt(user);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        user.Id,
                        user.FullName,
                        user.Role
                    }
                });
            }

        private string GenerateJwt(UserModel user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SUPER_SECRET_KEY_32_CHARS_MINIMUM")
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
