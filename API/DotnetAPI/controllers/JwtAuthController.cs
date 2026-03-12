using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

/// <summary>
/// Controls JSON Web Token creation.
/// </summary>
[ApiController]
[Route("[controller]")]
public class JWTController : ControllerBase
{
  /// <summary>
  /// Gets a token that expires in 15 minutes. A token is required to use the other endpoints.
  /// </summary>
  /// <remarks>
  /// For the sake of this project, there is only one Issuer and one Audience.
  /// 
  /// Since there are no inputs taken or checks on where requests come from, there is no reason to reject a request for a token.
  /// </remarks>
  /// <response code="200"> Generates and returns a token. </response>
  [HttpPost("token")]
  public IActionResult getToken()
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("a_totally_legitimate_key_to_use_here");
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Expires = DateTime.UtcNow.AddMinutes(15), // expires in just 15 minutes, so that expiration can be quickly witnessed/tested
      SigningCredentials = new SigningCredentials(
          new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
      Issuer = "http://localhost:5152/auth/token",
      Audience = "API user"
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var jwtToken = tokenHandler.WriteToken(token);

    return Ok(new { token = jwtToken });
  }
}