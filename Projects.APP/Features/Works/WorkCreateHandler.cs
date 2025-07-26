using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Projects.APP.Domain;
using Projects.APP.Services;
using System.ComponentModel.DataAnnotations;

namespace Projects.APP.Features.Works
{
    // Represents a request to create a new work item.
    public class WorkCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(300)]
        public string Name { get; set; } // The name of the work item (required, max length 300).

        public string Description { get; set; } // Optional description of the work item.

        public DateTime StartDate { get; set; } // The start date of the work item.

        public DateTime DueDate { get; set; } // The due date of the work item.

        public int? ProjectId { get; set; } // Optional project ID associated with the work item.
    }

    // Handles the creation of a new work item in the database.
    public class WorkCreateHandler : ProjectsDbService, IRequestHandler<WorkCreateRequest, CommandResponse>
    {
        // Constructor to initialize the database context.
        public WorkCreateHandler(ProjectsDb db) : base(db)
        {
        }

        // Handles the work creation request asynchronously.
        public async Task<CommandResponse> Handle(WorkCreateRequest request, CancellationToken cancellationToken)
        {
            // Validate that the due date is not earlier than the start date.
            if (request.DueDate < request.StartDate)
                return Error("Due date must be later or equal to start date!");

            // Check if a work item with the same name already exists in the database.
            if (await _db.Works.AnyAsync(w => w.Name.ToUpper() == request.Name.ToUpper().Trim()))
                return Error("Work with the same name exists!");

            // Create a new work entity.
            var entity = new Work()
            {
                Description = request.Description?.Trim(),
                DueDate = request.DueDate,
                Name = request.Name?.Trim(),
                ProjectId = request.ProjectId,
                StartDate = request.StartDate
            };

            // Add the new work item to the database and save changes.
            _db.Works.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Work created successfully.", entity.Id);
        }
    }
}
