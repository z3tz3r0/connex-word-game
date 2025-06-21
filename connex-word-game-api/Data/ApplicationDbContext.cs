using Microsoft.EntityFrameworkCore;
using MyProjectApi.Models.Entities;

namespace MyProjectApi.Data
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)

    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<WordScoring> WordScorings { get; set; }
  }
}