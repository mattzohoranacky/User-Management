namespace DotnetAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;

public partial class JwtAuthController(ProgramDbContext _context) : ControllerBase
{
  [HttpPost("login")]
  public IActionResult Login([FromBody] LoginModel model)
  {
    // Validate credentials (use a database in production)
    if (model.Username == "user" && model.Password == "password")
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: new[] { new Claim(ClaimTypes.Name, model.Username) },
          expires: DateTime.UtcNow.AddHours(1),
          signingCredentials: creds
      );

      return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
    return Unauthorized();
  }
}