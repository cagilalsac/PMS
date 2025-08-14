using Projects.APP.Domain;
using CORE.APP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Projects.APP.Services;

namespace Projects.APP.Features.Tags
{
    /// <summary>
    /// Represents a request to delete a tag containing Id property from the base Request class.
    /// </summary>
    public class TagDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    /// <summary>
    /// Handles the deletion of tags.
    /// </summary>
    public class TagDeleteHandler : ProjectsDbService, IRequestHandler<TagDeleteRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagDeleteHandler"/> class.
        /// </summary>
        /// <param name="db">The projects database context.</param>
        public TagDeleteHandler(ProjectsDb db) : base(db)
        {
        }

        /// <summary>
        /// Handles the tag deletion request.
        /// </summary>
        /// <param name="request">The tag deletion request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the command response.</returns>
        public async Task<CommandResponse> Handle(TagDeleteRequest request, CancellationToken cancellationToken)
        {
            // Before for only tag implementations: Find the tag to delete.
            //Tag tag = await _db.Tags.SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            // After project implementations: Find the tag including the relational project tags to delete.
            Tag tag = await _db.Tags.Include(t => t.ProjectTags).SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (tag is null)
                return Error("Tag not found!");

            // After project implementations: Remove the relational project tags.
            _db.ProjectTags.RemoveRange(tag.ProjectTags);

            // Delete the tag using one of the following ways:
            // Way 1: does not delete relational data (don't use)
            //_db.Entry(tag).State = EntityState.Deleted; 
            // Way 2: deletes relational data
            //_db.Remove(tag); 
            // Way 3: deletes relational data
            _db.Tags.Remove(tag); 

            // Save the changes to the database by unit of work.
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Tag deleted successfully.", tag.Id);
        }
    }
}
