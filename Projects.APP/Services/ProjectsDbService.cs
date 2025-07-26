using CORE.APP.Services;
using Projects.APP.Domain;
using System.Globalization;

namespace Projects.APP.Services
{
    /// <summary>
    /// Abstract base class for handling Projects database operations in the ProjectsDb context, inheriting from <see cref="ServiceBase"/>.
    /// </summary>
    public abstract class ProjectsDbService : ServiceBase
    {
        /// <summary>
        /// The ProjectsDb context used for Projects database operations.
        /// </summary>
        protected readonly ProjectsDb _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsDbService"/> class with the specified Projects database context.
        /// </summary>
        /// <param name="db">The ProjectsDb context to use for Projects database operations.</param>
        public ProjectsDbService(ProjectsDb db)
        {
            _db = db;

            // CultureInfo for English culture is set in the base constructor.
            // If another culture such as Turkish culture needs to be used, the assignment can be written as:
            //CultureInfo = new CultureInfo("tr-TR");
        }
    }
}
