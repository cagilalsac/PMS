using Projects.APP.Domain;
using CORE.APP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Projects.APP.Services;

namespace Projects.APP.Features.Tags
{
    /// <summary>
    /// Represents a request to update a tag.
    /// </summary>
    public class TagUpdateRequest : Request, IRequest<CommandResponse>
    {
        /// <summary>
        /// Gets or sets the name of the tag.
        /// The tag name is required, with a maximum length of 150 characters,
        /// and a minimum length of 3 characters.
        /// </summary>
        [Required, MaxLength(150), MinLength(3)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Handles the updating of tags.
    /// </summary>
    public class TagUpdateHandler : ProjectsDbService, IRequestHandler<TagUpdateRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagUpdateHandler"/> class.
        /// </summary>
        /// <param name="db">The projects database context.</param>
        public TagUpdateHandler(ProjectsDb db) : base(db)
        {
        }

        /// <summary>
        /// Handles the tag update request.
        /// </summary>
        /// <param name="request">The tag update request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the command response.</returns>
        public async Task<CommandResponse> Handle(TagUpdateRequest request, CancellationToken cancellationToken)
        {
            // Check if a tag with the same name already exists other than the record to be updated by using the Id condition.
            if (await _db.Tags.AnyAsync(t => t.Id != request.Id && t.Name.ToUpper() == request.Name.ToUpper().Trim(), cancellationToken))
                return Error("Tag with the same name exists!");

            // Retrieve the tag to update.
            // Way 1:
            //Tag tag = await _db.Tags.FindAsync(request.Id, cancellationToken);
            // Way 2:
            Tag tag = await _db.Tags.SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (tag is null)
                return Error("Tag not found!");

            // Update the tag name.
            tag.Name = request.Name.Trim();

            // Update the tag using one of the following ways:
            // Way 1: does not update relational data (don't use)
            //_db.Entry(tag).State = EntityState.Modified;
            // Way 2: updates relational data
            //_db.Update(tag);
            // Way 3: updates relational data
            _db.Tags.Update(tag);

            // Save the changes to the database by unit of work.
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Tag updated successfully.", tag.Id);
        }
    }
}
