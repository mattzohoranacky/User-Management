using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetAPI.Data;

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
            return _context.Users.ToList();
        }

        [Route("users/{Id}")]
        [HttpGet]
        public async Task<ActionResult<User>> GetUser(Guid Id)
        {
            var user = _context.Users.Find(Id);
            if (user == null)
            {
                return NotFound("This user does not exist.");
            }
            return user;
        }

        [Route("users")]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.Id = Guid.Empty;
            if (user.Name == "")
            {
                return BadRequest("Name cannot be empty.");
            }
            if (user.Email == "")
            {
                return BadRequest("Email cannot be empty.");
            }
            if (_context.Users.FirstOrDefault(u => u.Email == user.Email) != null)
            {
                return BadRequest("This email is already in use.");
            }
            _context.Users.Add(user);
            _context.Entry(user).State = EntityState.Added;
            _context.SaveChanges();
            return Created();
        }

        [Route("users/{Id}")]
        [HttpPut]
        public async Task<ActionResult<User>> PutUser(Guid Id, User user)
        {
            user.Id = Id; // easily ensure that user's Id matches, rather than newly generated Id; avoids rejecting Id in request body
            _context.Entry(user).State = EntityState.Modified; // updating UpdatedAt handled in the DbContext
            try
            {
                _context.Users.Update(user);
                _context.SaveChanges();
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
        public async Task<ActionResult<User>> DeleteUser(Guid Id)
        {
            var user = _context.Users.Find(Id);
            if (user == null)
            {
                return NotFound("Invalid user ID: This user does not exist.");
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok("Deleted user.");
        }
    }
}