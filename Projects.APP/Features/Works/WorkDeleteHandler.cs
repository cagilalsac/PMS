using Projects.APP.Domain;
using CORE.APP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Projects.APP.Services;

namespace Projects.APP.Features.Works
{
    // Represents a request to delete a work item.
    public class WorkDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    // Handles the deletion of a work item from the database.
    public class WorkDeleteHandler : ProjectsDbService, IRequestHandler<WorkDeleteRequest, CommandResponse>
    {
        // Constructor to initialize the database context.
        public WorkDeleteHandler(ProjectsDb db) : base(db)
        {
        }

        // Handles the work deletion request asynchronously.
        public async Task<CommandResponse> Handle(WorkDeleteRequest request, CancellationToken cancellationToken)
        {
            // Find the work item in the database by its ID.
            var entity = await _db.Works.SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            // If the work item is not found, return an error response.
            if (entity is null)
                return Error("Work not found!");

            // Remove the work item from the database.
            _db.Works.Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Work deleted successfully.", entity.Id);
        }
    }
}
