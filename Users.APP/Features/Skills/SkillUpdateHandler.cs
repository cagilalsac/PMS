using CORE.APP.Features;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;

namespace Users.APP.Features.Skills
{
    /// <summary>
    /// Represents a request to update an existing skill.
    /// </summary>
    public class SkillUpdateRequest : Request, IRequest<CommandResponse>
    {
        /// <summary>
        /// Gets or sets the name of the skill to be updated.
        /// </summary>
        /// <remarks>
        /// The name is required and must have a maximum length of 125 characters.
        /// </remarks>
        [Required, StringLength(125)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Handles the update of an existing skill in the system.
    /// </summary>
    public class SkillUpdateHandler : Service<Skill>, IRequestHandler<SkillUpdateRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillUpdateHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for handling skill-related operations.</param>
        public SkillUpdateHandler(UsersDb db) : base(db)
        {
        }

        /// <summary>
        /// Handles the request to update an existing skill.
        /// </summary>
        /// <param name="request">The request containing the skill update information.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="CommandResponse"/> indicating the success or failure of the skill update operation.</returns>
        public async Task<CommandResponse> Handle(SkillUpdateRequest request, CancellationToken cancellationToken)
        {
            // Check if another skill with the same name already exists
            if (await Query().AnyAsync(s => s.Id != request.Id && s.Name == request.Name, cancellationToken))
                return Error("Skill with the same name exists!");

            // Find the skill to be updated by its ID
            var skill = await Query().SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (skill is null)
                return Error("Skill not found!");

            // Update the skill's name
            skill.Name = request.Name?.Trim();

            // Mark the skill as updated and save changes
            await Update(skill, cancellationToken);

            return Success("Skill updated successfully.", skill.Id);
        }
    }
}
