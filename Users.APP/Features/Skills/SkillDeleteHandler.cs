using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;

namespace Users.APP.Features.Skills
{
    /// <summary>
    /// Represents a request to delete a skill from the system.
    /// </summary>
    public class SkillDeleteRequest : Request, IRequest<CommandResponse>
    {
        // No additional properties are required for this request.
    }

    /// <summary>
    /// Handles the deletion of a skill from the system.
    /// </summary>
    public class SkillDeleteHandler : Service<Skill>, IRequestHandler<SkillDeleteRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillDeleteHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for handling skill-related operations.</param>
        public SkillDeleteHandler(UsersDb db) : base(db) // DO NOT FORGET TO CHANGE THE CONSTRUCTOR'S PARAMETER from "DbContext db" to "UsersDb db"!
        {
        }

        /// <summary>
        /// Overrides the base Query method to retrieve Skill entities.
        /// </summary>
        /// <param name="isNoTracking">
        /// A boolean indicating whether Entity Framework tracking should be disabled (default is true).
        /// Disabling tracking is useful for read-only queries to improve performance and reduce memory usage.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{Skill}"/> representing a queryable collection of Skill entities.
        /// No additional related data is included, and it delegates the behavior to the base class implementation.
        /// </returns>
        protected override IQueryable<Skill> Query(bool isNoTracking = true)
        {
            // Delegates to the base class's Query method, which handles tracking configuration.
            // This method does not include any additional navigation properties or filters.
            return base.Query(isNoTracking).Include(s => s.UserSkills);
        }

        /// <summary>
        /// Handles the request to delete a skill from the system.
        /// </summary>
        /// <param name="request">The request containing the skill ID to be deleted.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="CommandResponse"/> indicating the success or failure of the skill deletion operation.</returns>
        public async Task<CommandResponse> Handle(SkillDeleteRequest request, CancellationToken cancellationToken)
        {
            // Find the skill by ID, including its associated user skills
            var skill = await Query().SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            // If the skill is not found, return an error response
            if (skill is null)
                return Error("Skill not found!");

            // Remove associated user skills before deleting the skill
            Delete(skill.UserSkills);

            // Delete the skill
            await Delete(skill, cancellationToken);

            return Success("Skill deleted successfully", skill.Id);
        }
    }
}
