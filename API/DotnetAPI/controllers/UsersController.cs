using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotnetAPI.Controllers
{
    [Route("~/")]
    [ApiController]
    public class UserController(ProgramDbContext _context) : ControllerBase
    {
        [Route("users")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [Route("users/{Id}")]
        [HttpGet]
        public async Task<ActionResult<User>> GetUser(Guid Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
            {
                return NotFound("This user does not exist.");
            }
            return user;
        }

        [Route("users")]
        [HttpPost]
        public async Task<ActionResult<StatusCodeHttpResult>> PostUser(User user)
        {
            if (user.Name == "" || user.Name == null)
            {
                return BadRequest("Users must include a name.");
            }
            if (user.Email == "" || user.Email == null)
            {
                return BadRequest("Users must include an email.");
            }
            if (_context.Users.Find(user.Email) != null)
            {
                return BadRequest("This email is already in use.");
            }
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Route("users/{Id}")]
        [HttpPut]
        public async Task<ActionResult<StatusCodeHttpResult>> PutUser(Guid Id, User user)
        {
            if (Id != user.Id)
            {
                return BadRequest("The provided ID and the provided user's ID do not match.");
            }
            user.UpdatedAt = DateTime.Now;
            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == Id))
                {
                    return NotFound("Invalid user ID: This user does not exist.");
                }
                else
                {
                    return StatusCode(500, new {Message = "Internal Error."});
                }
            }
            return Ok("Updated user.");
        }

        [Route("users/{Id}")]
        [HttpDelete]
        public async Task<ActionResult<StatusCodeHttpResult>> DeleteUser(Guid Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("Deleted user.");
        }
    }
}