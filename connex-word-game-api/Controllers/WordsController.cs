using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProjectApi.Data;
using MyProjectApi.Models.DTOs;
using MyProjectApi.Models.Entities;

namespace MyProjectApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class WordsController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public WordsController(ApplicationDbContext context)
    {
      _context = context;
    }

    // Post /api/words
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddWord([FromBody] AddWordDto dto)
    {
      if (!TryGetUserId(out var userId))
      {
        return Unauthorized();
      }

      var user = await _context.Users.FindAsync(userId);
      if (user == null)
      {
        return Unauthorized();
      }

      var todayCount = await _context.WordScorings
        .CountAsync(ws => ws.UserId == userId && ws.CreatedTime.Date == DateTime.UtcNow.Date);
      if (todayCount == 5)
      {
        return BadRequest(new { message = "You can only submit 5 words a day" });
      }

      var wordExists = await _context.WordScorings
        .AnyAsync(ws => ws.UserId == userId && ws.Word.ToLower() == dto.Word.ToLower());
      if (wordExists)
      {
        return BadRequest(new { message = "You have already submitted this word." });
      }

      int baseScore = CalculateScore(dto.Word);
      int finalScore = ApplyBonus(baseScore, user.IsVIP);

      var newWordScoring = new WordScoring
      {
        Word = dto.Word,
        Score = finalScore,
        CreatedTime = DateTime.UtcNow,
        ModifiedTime = DateTime.UtcNow,
        UserId = userId,
      };

      _context.WordScorings.Add(newWordScoring);
      await _context.SaveChangesAsync();

      var responseDto = new WordScoringDto
      {
        Id = newWordScoring.Id,
        Word = newWordScoring.Word,
        Score = newWordScoring.Score,
        CreateTime = newWordScoring.CreatedTime
      };

      return Ok(responseDto);

    }

    private bool TryGetUserId(out int userId)
    {
      var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userIdString))
      {
        userId = 0;
        return false;
      }
      return int.TryParse(userIdString, out userId);
    }

    private int CalculateScore(string word)
    {
      int score = 0;
      foreach (char c in word.ToLower())
      {
        switch (c)
        {
          case 'a': score += 2; break;
          case 'e': score += 3; break;
          case 'i': score += 4; break;
          case 'o': score += 5; break;
          case 'u': score += 6; break;
          default: score += 1; break;
        }
      }
      return score;
    }

    private int ApplyBonus(int baseScore, bool isVip)
    {
      var random = new Random();
      var score = baseScore;

      if (isVip)
      {
        if (random.Next(1, 101) <= 10)
        {
          score *= 2;
        }
      }

      else if (random.Next(1, 101) <= 5)
      {
        return score *= 2;
      }

      return score;
    }

    // GET /api/words/history
    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> GetHistory()
    {
      if (!TryGetUserId(out var userId))
      {
        return Unauthorized();
      }

      var history = await _context.WordScorings
        .Where(ws => ws.UserId == userId)
        .OrderByDescending(ws => ws.CreatedTime)
        .Select(ws => new WordScoringDto
        {
          Id = ws.Id,
          Word = ws.Word,
          Score = ws.Score,
          CreateTime = ws.CreatedTime
        }).ToListAsync();
      return Ok(history);
    }

    [HttpGet("top5")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTop5()
    {
      var topPlayers = await _context.Users
        .OrderByDescending(u => u.WordScorings.Sum(ws => ws.Score))
        .Take(5)
        .Select(u => new TopPlayerDto
        {
          Username = u.Username,
          TotalScore = u.WordScorings.Sum(ws => ws.Score)
        }).ToListAsync();
      return Ok(topPlayers);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "VipOnly")]
    public async Task<IActionResult> EditWord(int id, [FromBody] EditWordDto dto)
    {
      if (!TryGetUserId(out var userId))
      {
        return Unauthorized();
      }

      var wordScoring = await _context.WordScorings.FindAsync(id);

      if (wordScoring == null) return NotFound();
      if (wordScoring.UserId != userId) return Forbid();

      await _context.SaveChangesAsync();

      var responseDto = new WordScoringDto
      {
        Id = wordScoring.Id,
        Word = wordScoring.Word,
        Score = wordScoring.Score,
        CreateTime = wordScoring.CreatedTime
      };

      return Ok(responseDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "VipOnly")]
    public async Task<IActionResult> DeleteWord([FromRoute] int id)
    {
      var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (!TryGetUserId(out var userId))
      {
        return Unauthorized();
      }

      var wordScoring = await _context.WordScorings.FindAsync(id);
      if (wordScoring == null) return NotFound();
      if (wordScoring.UserId != userId) return Forbid();

      _context.WordScorings.Remove(wordScoring);
      await _context.SaveChangesAsync();
      return NoContent();
    }
  }
}