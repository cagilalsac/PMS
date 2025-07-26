using Users.APP.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Users.API.Controllers
{
    /// <summary>
    /// Controller responsible for direct database operations for seeding user-related entities.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersDbController : ControllerBase
    {
        private readonly UsersDb _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersDbController"/> class.
        /// </summary>
        /// <param name="db">The user database context.</param>
        public UsersDbController(UsersDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Seeds the database with initial Roles, Users, and Skills.
        /// WARNING: This will clear and reset all existing Users, Roles, Skills, and UserSkills data.
        /// </summary>
        /// <returns>Returns a success message if seeding completes successfully.</returns>
        [HttpGet("Seed")]
        public IActionResult Seed()
        {
            // Remove existing records from UserSkills, Skills, Users, and Roles tables
            var userSkills = _db.UserSkills.ToList();
            _db.UserSkills.RemoveRange(userSkills);
            var skills = _db.Skills.ToList();
            _db.Skills.RemoveRange(skills);
            var users = _db.Users.ToList();
            _db.Users.RemoveRange(users);
            var roles = _db.Roles.ToList();
            _db.Roles.RemoveRange(roles);

            // Reset identity columns if tables were not empty for SQL Server
            //if (users.Any() || roles.Any() || userSkills.Any() || skills.Any())
            //{
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Users', RESEED, 0)");
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Roles', RESEED, 0)");
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Skills', RESEED, 0)");
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('UserSkills', RESEED, 0)");
            //}

            // Add default skills
            _db.Skills.Add(new Skill { Name = "Developer" });
            _db.Skills.Add(new Skill { Name = "Instructor" });

            _db.SaveChanges(); // Commit skills to retrieve their IDs later

            // Add "Admin" role with one associated user
            _db.Roles.Add(new Role
            {
                Name = "Admin",
                Users = new List<User>
                {
                    new User
                    {
                        IsActive = true,
                        Name = "Çağıl",
                        Surname = "Alsaç",
                        UserName = "admin",
                        Password = "admin",
                        RegistrationDate = new DateTime(2025, 01, 13),
                        UserSkills = new List<UserSkill>
                        {
                            new UserSkill
                            {
                                SkillId = _db.Skills.SingleOrDefault(s => s.Name == "Developer").Id
                            },
                            new UserSkill
                            {
                                SkillId = _db.Skills.SingleOrDefault(s => s.Name == "Instructor").Id
                            }
                        }
                    }
                }
            });

            // Add "User" role with one associated user
            _db.Roles.Add(new Role
            {
                Name = "User",
                Users = new List<User>
                {
                    new User
                    {
                        IsActive = true,
                        Name = "Leo",
                        Surname = "Alsaç",
                        UserName = "user",
                        Password = "user",
                        RegistrationDate = DateTime.Parse("01/24/2025", new CultureInfo("en-US"))
                    }
                }
            });

            _db.SaveChanges();

            return Ok("Database seed successful.");
        }
    }
}
