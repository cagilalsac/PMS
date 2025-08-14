using Microsoft.AspNetCore.Mvc;
using Users.APP.Domain;

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
        /// Seeds the database with initial Roles, Users, UserRoles and UserDetails.
        /// WARNING: This will clear and reset all existing Users, Roles, UserRoles and UserDetails data.
        /// </summary>
        /// <returns>Returns a success message if seeding completes successfully.</returns>
        [HttpGet("Seed")]
        public IActionResult Seed()
        {
            // Remove existing records from UserRoles, UserDetails, Users, and Roles tables
            var userRoles = _db.UserRoles.ToList();
            _db.UserRoles.RemoveRange(userRoles);
            var userDetails = _db.UserDetails.ToList();
            _db.UserDetails.RemoveRange(userDetails);
            var users = _db.Users.ToList();
            _db.Users.RemoveRange(users);
            var roles = _db.Roles.ToList();
            _db.Roles.RemoveRange(roles);

            // Reset identity columns if tables were not empty for SQL Server
            //if (users.Any() || roles.Any() || userRoles.Any() || userDetails.Any())
            //{
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Users', RESEED, 0)");
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Roles', RESEED, 0)");
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('UserRoles', RESEED, 0)");
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('UserDetails', RESEED, 0)");
            //}

            // Add roles
            _db.Roles.Add(new Role()
            {
                Name = "Admin"
            });

            _db.Roles.Add(new Role()
            {
                Name = "User"
            });

            // Commit role inserts to the database
            _db.SaveChanges();

            // Add users with user details and roles
            _db.Users.Add(new User()
            { 
                IsActive = true,
                UserName = "admin",
                Password = "admin",
                UserRoles = new List<UserRole>()
                {
                    new UserRole()
                    {
                        RoleId = _db.Roles.SingleOrDefault(r => r.Name == "Admin").Id
                    }
                },
                UserDetails = new List<UserDetail>()
                {
                    new UserDetail()
                    {
                        Phone = "5321234567",
                        Email = "admin@pms.com",
                        Address = "Çankaya, Ankara"
                    }
                }
            });

            _db.Users.Add(new User()
            {
                IsActive = true,
                UserName = "user",
                Password = "user",
                UserRoles = new List<UserRole>()
                {
                    new UserRole()
                    {
                        RoleId = _db.Roles.SingleOrDefault(r => r.Name == "User").Id
                    }
                }
            });

            // Commit user with user details and user roles inserts to the database
            _db.SaveChanges();

            return Ok("Database seed successful.");
        }
    }
}
