using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
  public class ProgramDbContext : DbContext
  {
    public ProgramDbContext(DbContextOptions<ProgramDbContext> options) : base(options)
    {}
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
        .HasIndex(u => u.Email).IsUnique();
    }

    public override int SaveChanges()
    {
      var userEntries = ChangeTracker
        .Entries()
        .Where(e => e.Entity is BaseUser && (
            e.State == EntityState.Added || e.State == EntityState.Modified));
      var now = DateTime.Now;

      foreach (var userEntry in userEntries)
      {
        if (userEntry.State == EntityState.Added)
        {
        now = DateTime.Now;
          ((BaseUser)userEntry.Entity).CreatedAt = now;
          ((BaseUser)userEntry.Entity).UpdatedAt = now;
        } else
        {
        now = DateTime.Now;
          ((BaseUser)userEntry.Entity).UpdatedAt = now;
        }
      }
      return base.SaveChanges();
    }
  }
}