using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Users.APP.Domain
{
    /// <summary>
    /// Represents the database context for managing users, roles, skills, and user skills.
    /// </summary>
    public class UsersDb : DbContext
    {
        /// <summary>
        /// Gets or sets the Users table.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the Roles table.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets the Skills table.
        /// </summary>
        public DbSet<Skill> Skills { get; set; }

        /// <summary>
        /// Gets or sets the UserSkills table, which represents the many-to-many relationship between users and skills.
        /// </summary>
        public DbSet<UserSkill> UserSkills { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersDb"/> class using the provided database options.
        /// </summary>
        /// <param name="options">The database context options.</param>
        public UsersDb(DbContextOptions options) : base(options)
        {
        }

        // The connection string should be defined in appsettings.json of the API Project.
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // SQLite:
        //    base.OnConfiguring(optionsBuilder.UseSqlite("data source=PMSUsersDB"));

        //    // SQL Server LocalDB:
        //    base.OnConfiguring(optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=PMSUsersDB;trusted_connection=true;"));
        //}
    }



    /// <summary>
    /// Factory class for creating instances of <see cref="UsersDb"/> at design time
    /// for solving problems about API scaffolding.
    /// </summary>
    public class UsersDbFactory : IDesignTimeDbContextFactory<UsersDb>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UsersDb"/> database context.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>A new instance of <see cref="UsersDb"/>.</returns>
        public UsersDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UsersDb>();

            // Configure the database connection
            // Change the connection string based on your environment

            // SQLite:
            optionsBuilder.UseSqlite("data source=PMSUsersDB");

            // SQL Server LocalDB:
            //optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=PMSUsersDB;trusted_connection=true;");

            return new UsersDb(optionsBuilder.Options);
        }
    }
}
