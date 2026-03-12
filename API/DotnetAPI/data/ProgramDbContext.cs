using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
  /// <summary>
  /// The Entity Framework Core DbContext of this API.
  /// </summary>
  public class ProgramDbContext : DbContext
  {
    /// <summary>
    /// Constructor for a ProgramDbContext.
    /// </summary>
    /// <param name="options"></param>
    public ProgramDbContext(DbContextOptions<ProgramDbContext> options) : base(options)
    {}

    /// <summary>
    /// A DbSet of User entities, used for querying and saving users.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Overrides the default OnModelCreating method to enforce unique email (has no effect on the in-memory database).
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
        .HasIndex(u => u.Email).IsUnique();
    }

    /// <summary>
    /// Overrides the default SaveChanges method to generate the CreatedAt and UpdatedAt values.
    /// </summary>
    /// <returns></returns>
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