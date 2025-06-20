using System.Collections.Generic;

namespace MyProjectApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsVIP { get; set; }

        // การประกาศ ICollection เพื่อบอกว่า User จะมีข้อมูลแบบ WordScoring หลายอันนะ
        public ICollection<WordScoring> WordScorings { get; set; } = new List<WordScoring>();
    }
}