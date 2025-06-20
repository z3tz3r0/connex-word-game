using System;

namespace MyprojectApi.Models
{
  public class WordScoring
  {
    public int UserId { get; set; }

    public string Word { get; set; }

    public int Score { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTime ModifiedTime { get; set; }

    public int UserId { get; set; }

    public User User { get; set; }

  }
}