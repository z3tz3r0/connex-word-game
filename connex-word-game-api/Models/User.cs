using System.Collections.Generic;

namespace MyProjectApi.Models
{
  public class User
  {
    public int Id { get; set; }
    public string Username { get; set; }

    public string Password { get; set; }

    public bool isVip { get; set; }

    public ICollection<WordScoring> WordScorings { get; set; } = new List<WordScoring>();
  }
}