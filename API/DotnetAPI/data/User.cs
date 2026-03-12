using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
  /// <summary>
  /// An entity that represents a single user.
  /// </summary>
  [Index(nameof(Email), IsUnique = true)]
  public class User : BaseUser
  {
    /// <summary>
    /// The ID of the user. Specifically, a Guid.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    /// <summary>
    /// The name of the user. Maximum of 100 characters long.
    /// </summary>
    [StringLength(100)]
    public required string Name { get; set; }
    /// <summary>
    /// The email address of the user.
    /// </summary>
    [EmailAddress]
    public required string Email { get; set; }
    /// <summary>
    /// The date of birth of the user.
    /// </summary>
    public DateTime DateOfBirth { get; set;}
  }

  /// <summary>
  /// A base class for users, used for generating CreatedAt and UpdatedAt values.
  /// </summary>
  public class BaseUser
  {
    /// <summary>
    /// The date and time that the user object was created.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt {get; set;}
    /// <summary>
    /// The date and time that the user object was updated.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt {get; set;}
  }
}