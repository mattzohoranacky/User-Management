using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
  public class ProgramDbContext : DbContext
  {
    public ProgramDbContext(DbContextOptions<ProgramDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
  }
}