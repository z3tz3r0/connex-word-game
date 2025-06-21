using System;

namespace MyProjectApi.Models.Entities
{
    public class WordScoring
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}