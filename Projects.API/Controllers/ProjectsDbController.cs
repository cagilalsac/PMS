#nullable disable
using Projects.APP.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Generated from Custom Template.
namespace Projects.API.Controllers
{
    /// <summary>
    /// Controller for managing direct database operations related to projects, primarily for seeding data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsDbController : ControllerBase
    {
        private readonly ProjectsDb _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsDbController"/> class with a database context.
        /// </summary>
        /// <param name="db">The database context for accessing project-related entities.</param>
        public ProjectsDbController(ProjectsDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Seeds the database with default project, tag, and work data.
        /// WARNING: This method deletes all existing data in Projects, Works, Tags, and ProjectTags tables.
        /// </summary>
        /// <returns>An HTTP 200 response if seeding is successful.</returns>
        [HttpGet("Seed")]
        public IActionResult Seed()
        {
            // Remove all existing data from related tables
            var projectTags = _db.ProjectTags.ToList();
            _db.ProjectTags.RemoveRange(projectTags);
            var tags = _db.Tags.ToList();
            _db.Tags.RemoveRange(tags);
            var works = _db.Works.ToList();
            _db.Works.RemoveRange(works);
            var projects = _db.Projects.ToList();
            _db.Projects.RemoveRange(projects);

            // Reset identity columns if any data was removed for SQL Server
            //if (projectTags.Any() || tags.Any() || works.Any() || projects.Any())
            //{
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ProjectTags', RESEED, 0)");
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Tags', RESEED, 0)");
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Works', RESEED, 0)");
            //    _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Projects', RESEED, 0)");
            //}

            // Add sample tags
            _db.Tags.AddRange(
                new Tag() { Name = "C#" },
                new Tag() { Name = "Object Oriented Programming" },
                new Tag() { Name = "ASP.NET" },
                new Tag() { Name = "Entity Framework" },
                new Tag() { Name = "Microservices" },
                new Tag() { Name = "MVC" },
                new Tag() { Name = "Clean Architecture" },
                new Tag() { Name = "N-Layered Architecture" }
            );

            _db.SaveChanges(); // Save tags before referencing them by ID

            // Add first sample project
            _db.Projects.Add(new Project()
            {
                Description = "Bilkent University Computer Technology and Information Systems CTIS 465 Lecture",
                Name = "Bilkent CTIS 465 Spring 2025",
                ProjectTags = new List<ProjectTag>
                {
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "C#").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "Object Oriented Programming").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "ASP.NET").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "Entity Framework").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "Microservices").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "Clean Architecture").Id }
                },
                Works = new List<Work>
                {
                    new Work
                    {
                        Name = "Preperation of lecture notes",
                        StartDate = DateTime.Now.AddMonths(-12),
                        DueDate = DateTime.Now.AddMonths(-8)
                    },
                    new Work
                    {
                        Name = "Preperation of lecture project demo",
                        StartDate = DateTime.Now.AddMonths(-7),
                        DueDate = DateTime.Now.AddMonths(-1)
                    }
                }
            });

            // Add second sample project
            _db.Projects.Add(new Project()
            {
                Description = "Bilkent University Computer Technology and Information Systems CTIS 479 Lecture",
                Name = "Bilkent CTIS 479 Fall 2025",
                ProjectTags = new List<ProjectTag>
                {
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "C#").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "Object Oriented Programming").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "ASP.NET").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "Entity Framework").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "MVC").Id },
                    new ProjectTag { TagId = _db.Tags.SingleOrDefault(t => t.Name == "N-Layered Architecture").Id }
                },
                Works = new List<Work>
                {
                    new Work
                    {
                        Name = "Preperation of lecture notes",
                        StartDate = DateTime.Now.AddMonths(-24),
                        DueDate = DateTime.Now.AddMonths(-20)
                    },
                    new Work
                    {
                        Name = "Preperation of lecture project demo",
                        StartDate = DateTime.Now.AddMonths(-18),
                        DueDate = DateTime.Now.AddMonths(-16)
                    },
                    new Work
                    {
                        Name = "Lecture project demo publish",
                        StartDate = DateTime.Now.AddMonths(-15),
                        DueDate = DateTime.Now.AddMonths(-14)
                    }
                }
            });

            _db.SaveChanges();

            return Ok("Database seed successful.");
        }
    }
}
