namespace MyProjectApi.Models.DTOs
{
  public class WordScoringDto
  {
    public int Id { get; set; }
    public string Word { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime CreateTime { get; set; }
  }
}