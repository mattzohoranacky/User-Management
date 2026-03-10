using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetAPI.Data
{
  public class User : BaseUser
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Name { get; set; }
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