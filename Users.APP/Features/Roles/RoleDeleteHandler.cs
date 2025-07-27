using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;

namespace Users.APP.Features.Roles
{
    /// <summary>
    /// Represents a request to delete a role.
    /// </summary>
    public class RoleDeleteRequest : Request, IRequest<CommandResponse>
    {
        // No additional properties are required for this request.
    }

    /// <summary>
    /// Handles the deletion of a role in the system.
    /// </summary>
    public class RoleDeleteHandler : Service<Role>, IRequestHandler<RoleDeleteRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleDeleteHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for handling role-related operations.</param>
        public RoleDeleteHandler(UsersDb db) : base(db)
        {
        }

        /// <summary>
        /// Overrides the base Query method to include related Users when querying Roles.
        /// </summary>
        /// <param name="isNoTracking">
        /// A boolean value indicating whether the query should be tracked by the context (default is true).
        /// When set to true, the query operates in "no-tracking" mode, which can improve performance for read-only operations.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{Role}"/> that includes the Users navigation property,
        /// allowing the retrieval of Role entities along with their associated User entities.
        /// </returns>
        protected override IQueryable<Role> Query(bool isNoTracking = true)
        {
            // Call the base implementation of Query, optionally disabling tracking for better performance on reads.
            // Then use Include to eagerly load the Users related to each Role.
            return base.Query(isNoTracking).Include(r => r.Users);
        }

        /// <summary>
        /// Handles the request to delete a role.
        /// </summary>
        /// <param name="request">The request containing the role ID to be deleted.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="CommandResponse"/> indicating the success or failure of the role deletion operation.</returns>
        public async Task<CommandResponse> Handle(RoleDeleteRequest request, CancellationToken cancellationToken)
        {
            // Retrieve the role with its related users
            var role = await Query().SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            // If the role is not found, return an error
            if (role is null)
                return Error("Role not found!");

            // If the role has associated users, it cannot be deleted
            // Way 1:
            //if (role.Users.Count > 0)
            // Way 2:
            if (role.Users.Any())
                return Error("Role can't be deleted because it has relational users!");

            // Remove the role and save changes
            await Delete(role, cancellationToken);

            return Success("Role deleted successfully", role.Id);
        }
    }
}
