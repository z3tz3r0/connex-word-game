using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProjectApi.Data;
using MyProjectApi.Models;
using System.Threading.Tasks;

namespace MyProjectApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
      _context = context;
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

      return Ok(new {message = "User registered successfully"});
    }
  }
}