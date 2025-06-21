namespace MyProjectApi.Models.DTOs
{
  public class TopPlayerDto
  {
    public string Username { get; set; } = string.Empty;
    public required int TotalScore { get; set; }
  }
}