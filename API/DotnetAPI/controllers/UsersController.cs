using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetAPI.Data;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace DotnetAPI.Controllers
{
    /// <summary>
    /// Controls all User-related requests.
    /// </summary>
    /// <param name="_context"></param>
    [Authorize]
    [Route("~/")]
    [ApiController]
    public class UserController(ProgramDbContext _context) : ControllerBase
    {
        /// <summary>
        /// Retrieves a list of up to 5 users that fit the criteria defined by the URL parameters.
        /// </summary>
        /// <remarks>
        /// The next 5 users are retrieved on the next page.
        /// 
        /// URL Parameters:
        /// 
        ///     sortBy
        ///         Valid options are: name:asc, name:desc, email:asc, email:desc, age:asc, age:desc.
        ///         Users are, by default, sorted by name in ascending order.
        /// 
        ///     name
        ///         "name=John" will retrieve users whose names include "John".
        ///         "name=John:exact" will retrieve users whose names are "John".
        /// 
        ///     email
        ///         "email=@gmail" will retrieve users whose emails include "@gmail".
        ///         "email=email@gmail.com:exact" will retrieve users whose emails are "email@gmail.com"
        ///         Since user emails are unique, using ':exact' will only retrieve a single user.
        /// 
        ///     age
        ///         "age=20" will retrieve users that are 20 years old.
        ///         "age=[20+TO+23]" will retrieve users that are between 20 years old and 23 years old, inclusive.
        /// 
        ///     p
        ///         "p=1" will retrieve the first page of users.
        ///         "p=0" will be treated as "p=1".
        /// 
        /// </remarks>
        /// <response code="200"> Returns a list of users. </response>
        /// <response code="400"> Provides a message about what input was invalid and how to properly use it. </response>
        /// <response code="500"> Produces an error. </response>
        [Route("users")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            string sortBy = Request.Query["sortBy"]!;
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "name:asc";
            }
            StringBuilder nameFilter = new StringBuilder(Request.Query["name"]);
            StringBuilder emailFilter =  new StringBuilder(Request.Query["email"]);
            string ageFilter = Request.Query["age"]!;
            string page = Request.Query["p"]!;
            IQueryable<User> set = _context.Users;
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
            if (!string.IsNullOrEmpty(nameFilter.ToString()))
            {
                if (!Regex.IsMatch(nameFilter.ToString(), @".+:exact$"))
                {
                    set = set.Where(u => u.Name.Contains(nameFilter.ToString()));
                } else
                {
                    nameFilter.Remove(nameFilter.Length-6, 6);
                    set = set.Where(u => u.Name.Equals(nameFilter.ToString()));
                    Console.WriteLine(nameFilter.ToString());
                }
            }
            if (!string.IsNullOrEmpty(emailFilter.ToString()))
            {
                if (!Regex.IsMatch(emailFilter.ToString(), @".+:exact$"))
                {
                    set = set.Where(u => u.Email.Contains(emailFilter.ToString()));
                } else
                {
                    emailFilter.Remove(emailFilter.Length-6, 6);
                    set = set.Where(u => u.Email.Equals(emailFilter.ToString()));
                    Console.WriteLine(emailFilter.ToString());
                }
            }
            if (!string.IsNullOrEmpty(ageFilter))
            {
                if (Regex.IsMatch(ageFilter, @"^[\[]\d{1,3}[ ]TO[ ]\d{1,3}[\]]$"))
                {
                    try
                    {
                        string[] ageFilterParts = ageFilter.Split("TO");
                        int ageMin = Convert.ToInt16(Regex.Match(ageFilterParts[0], @"\d{1,3}").Value);
                        int ageMax = Convert.ToInt16(Regex.Match(ageFilterParts[1], @"\d{1,3}").Value);
                        set = set.Where(u => u.DateOfBirth >= DateTime.Today.AddYears(-ageMax-1).AddDays(1)
                            && u.DateOfBirth <= DateTime.Today.AddYears(-ageMin));
                    } catch (Exception e)
                    {
                        return StatusCode(500, "Sorting failed due to exception: " + e.Message);
                    }
                } else if (Regex.IsMatch(ageFilter, @"^\d{1,3}$"))
                {
                    int age = Convert.ToInt16(Regex.Match(ageFilter, @"^\d{1,3}$").Value);
                    set = set.Where(u => u.DateOfBirth >= DateTime.Today.AddYears(-age-1).AddDays(1)
                        && u.DateOfBirth <= DateTime.Today.AddYears(-age));
                } else
                {
                    return BadRequest("Invalid format for value of \"age\". Valid formats:\n" +
                        "\t\"[#+TO+#]\" to get all users within a range of ages (\"age=[18+TO+20]\")\n" +
                        "\t\"#\" to get all users of a certain age (\"age=18\")");
                }
            }
            if (!string.IsNullOrEmpty(page))
            {
                if (!Regex.IsMatch(page, @"^\d{1,8}$"))
                {
                    return BadRequest("Invalid format for value of page \"p\". Page must be a positive integer of up to 8 digits.\n\n" +
                    "\"http://localhost:5152/users?p=1\" will retrieve the first page.\n" +
                    "If the page requested is less than 1, the first page will be displayed.");
                }
                int PageNumber = Convert.ToInt32(Regex.Match(page, @"^\d{1,8}$").Value);
                if (PageNumber < 1)
                {
                    PageNumber = 1;
                }
                var value = await set.Skip((PageNumber-1) * PageLength).Take(PageLength).ToListAsync();
                return value;
            } else
            {
                var value = await set.Take(PageLength).ToListAsync();
                return value;
            }
        }

        /// <summary>
        /// Gets a specific user by ID.
        /// </summary>
        /// <param name="Id"> The ID of the user to get. </param>
        /// <response code="200"> Returns the user. </response>
        /// <response code="404"> Provides a message stating that the user does not exist. </response>
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

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <remarks>
        /// The Id, CreatedAt, and UpdatedAt values are generated.
        /// These can safely be left out of the request body.
        /// Adding these as part of the request body will not impact the generated values.
        /// 
        /// The user's date of birth can be in date format (YYYY-MM-DD) or DateTime format.
        /// 
        /// Sample Request:
        /// 
        ///     POST users/7aba7140-b70a-44e5-b7f3-f483c98aad30
        ///     {
        ///         "name": "John",
        ///         "email": "john@gmail.com",
        ///         "dateOfBirth": "1983-03-10"
        ///     }
        /// </remarks>
        /// <param name="user"> The user information to be inserted. </param>
        /// <response code="201"> Creates the user. </response>
        /// <response code="400"> Refuses to create the user and provides a message describing why. </response>
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

        /// <summary>
        /// Updates a specific user's details.
        /// </summary>
        /// <remarks>
        /// The Id, CreatedAt, and UpdatedAt values are generated and cannot be updated via this request.
        /// These can safely be left out of the request body.
        /// Adding these as part of the request body will not impact the generated values.
        /// 
        /// The user's date of birth can be in date format (YYYY-MM-DD) or DateTime format.
        /// 
        /// Sample Request:
        /// 
        ///     PUT users/7aba7140-b70a-44e5-b7f3-f483c98aad30
        ///     {
        ///         "name": "Bob",
        ///         "email": "bob@live.com",
        ///         "dateOfBirth": "2008-03-10"
        ///     }
        /// </remarks>
        /// <param name="Id"> The ID of the user to update. </param>
        /// <param name="user"> The user information to be inserted. </param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes a specific user by ID.
        /// </summary>
        /// <param name="Id"> The ID of the user to be deleted. </param>
        /// <response code="200"> Deletes the user. </response>
        /// <response code="404"> Provides a message stating that a user with that ID does not exist. </response>
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
        private static int PageLength = 5; // small page length/size so pagination can be seen with few users
    }
}