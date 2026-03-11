using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;

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
            string sortBy = Request.Query["sortBy"]!;
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "name:asc";
            }
            string nameFilter = Request.Query["name"]!;
            string emailFilter = Request.Query["email"]!;
            string ageFilter = Request.Query["age"]!;
            IQueryable<User> set = _context.Users;
            if (!string.IsNullOrEmpty(nameFilter))
            {
                set = set.Where(u => u.Name.Contains(nameFilter));
            }
            if (!string.IsNullOrEmpty(emailFilter))
            {
                set = set.Where(u => u.Email.Contains(emailFilter));
            }
            if (!string.IsNullOrEmpty(ageFilter))
            {
                if (Regex.IsMatch(ageFilter, @"^[\[]\d*[ ]TO[ ]\d*[\]]$"))
                {
                    try
                    {
                        string[] ageFilterParts = ageFilter.Split("TO");
                        int ageMin = Convert.ToInt16(Regex.Match(ageFilterParts[0], @"\d+").Value);
                        int ageMax = Convert.ToInt16(Regex.Match(ageFilterParts[1], @"\d+").Value);
                        set = set.Where(u => u.DateOfBirth >= DateTime.Today.AddYears(-ageMin));
                        set = set.Where(u => u.DateOfBirth <= DateTime.Today.AddYears(-ageMax+1).AddSeconds(1));
                    } catch (Exception e)
                    {
                        return StatusCode(500, "Sorting failed due to exception: " + e.Message);
                    }
                } else if (Regex.IsMatch(ageFilter, @"^\d*$"))
                {
                    int age = Convert.ToInt16(Regex.Match(ageFilter, @"\d+").Value);
                    set = set.Where(u => u.DateOfBirth >= DateTime.Today.AddYears(-age-1).AddSeconds(1)
                        && u.DateOfBirth <= DateTime.Today.AddYears(-age));
                } else
                {
                    return BadRequest("Invalid format for value of \"age\". Valid formats:\n" +
                        "\t\"[#+TO+#]\" to get all users within a range of ages (\"age=[18+TO+20]\")\n" +
                        "\t\"#\" to get all users of a certain age (\"age=18\")");
                }
            }
            switch (sortBy)
            {
                case "name:asc":
                    set = set.OrderBy(u => u.Name);
                    break;
                case "name:desc":
                    set = set.OrderByDescending(u => u.Name);
                    break;
                case "email:asc":
                    set = set.OrderBy(u => u.Email);
                    break;
                case "email:desc":
                    set = set.OrderByDescending(u => u.Email);
                    break;
                case "age:asc":
                    set = set.OrderByDescending(u => u.DateOfBirth);
                    break;
                case "age:desc":
                    set = set.OrderBy(u => u.DateOfBirth);
                    break;
                default:
                    return BadRequest("Invalid sorting option: \"" + sortBy + 
                            "\".\nValid sorting options include:\n" +
                            "\t\"name:asc\"\n" +
                            "\t\"name:desc\"\n" + 
                            "\t\"email:asc\"\n" +
                            "\t\"email:desc\"\n" +
                            "\t\"age:asc\"\n" +
                            "\t\"age:desc\"");
            }
            return set.ToList();
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
            if (user.DateOfBirth.AddYears(18) > DateTime.Now)
            {
                return BadRequest("User must be at least 18 years old.");
            }
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
            if (_context.Users.FirstOrDefault(u => u.Email == user.Email && u.Id != Id) != null)
            {
                return BadRequest("This email is already in use.");
            }
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