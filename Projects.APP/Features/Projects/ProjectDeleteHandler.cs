using Projects.APP.Domain;
using CORE.APP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Projects.APP.Services;

namespace Projects.APP.Features.Projects
{
    /// <summary>
    /// Represents a request to delete a project.
    /// </summary>
    public class ProjectDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    /// <summary>
    /// Handles the deletion of a project.
    /// </summary>
    public class ProjectDeleteHandler : ProjectsDbService, IRequestHandler<ProjectDeleteRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDeleteHandler"/> class.
        /// </summary>
        /// <param name="db">Database context for projects.</param>
        public ProjectDeleteHandler(ProjectsDb db) : base(db)
        {
        }

        /// <summary>
        /// Handles the request to delete a project.
        /// </summary>
        /// <param name="request">The project deletion request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A command response indicating success or failure.</returns>
        public async Task<CommandResponse> Handle(ProjectDeleteRequest request, CancellationToken cancellationToken)
        {
            // Retrieve the project entity along with its associated works and tags
            var entity = await _db.Projects
                .Include(p => p.Works)
                .Include(p => p.ProjectTags)
                .SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            // Return an error if the project does not exist
            if (entity is null)
                return Error("Project not found!");

            // do not delete the project if there are any relational works data
            if (entity.Works.Count > 0) // if (entity.Works.Any()) can also be written
                return Error("Project can't be deleted because it has relational works!");

            // Remove all associated tags before deleting the project
            _db.ProjectTags.RemoveRange(entity.ProjectTags);

            // Remove the project from the database
            _db.Projects.Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Project deleted successfully.", entity.Id);
        }
    }
}
