using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using MyProjectApi.Data;
using MyProjectApi.Models.Entities;
using MyProjectApi.Models.DTOs;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace MyProjectApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UsersController(ApplicationDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    // POST /api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
      var userExists = await _context.Users.AnyAsync(x => x.Username == user.Username);
      if (userExists)
      {
        return BadRequest(new { message = "Username already exists." });
      }
      user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      return Ok(new { message = "User registered successfully" });
    }

    // POST /api/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

      if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
      {
        return Unauthorized(new { message = "Invalid Username or Password" });
      }

      // ไปเรียก Method ที่เราสร้างไว้ด้านล่างนี้มาใช้ในการสร้าง Jwt Token
      var token = GenerateJwtToken(user);

      return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
      // ไปเรียกเอา Key จาก appsettings.json
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      // สร้าง Claim ใหม่ (เหมือนจะเป็น package ไว้แนบข้อมูลบางอย่างไว้กับ JWT ได้)
      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Name, user.Username),
        new Claim("isVip", user.IsVIP.ToString())
      };

      // สร้าง Token แล้วตั้งค่าต่าง ๆ
      var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }


    // GET /api/users/profile
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (userId == null)
      {
        return NotFound();
      }

      var user = await _context.Users.FindAsync(int.Parse(userId));

      if (user == null)
      {
        return NotFound();
      }

      return Ok(new { user.Id, user.Username, user.IsVIP });
    }
  }
}