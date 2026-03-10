using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
  [Index(nameof(Email), IsUnique = true)]
  public class User : BaseUser
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [StringLength(100)]
    public required string Name { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public DateTime DateOfBirth { get; set;}
  }

  public class BaseUser
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt {get; set;}
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt {get; set;}
  }
}