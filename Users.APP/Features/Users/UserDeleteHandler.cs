using Users.APP.Domain;
using CORE.APP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Services;

namespace Users.APP.Features.Users
{
    /// <summary>
    /// Represents a request to delete a user.
    /// </summary>
    public class UserDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    /// <summary>
    /// Handles the deletion of a user from the system.
    /// </summary>
    public class UserDeleteHandler : UserService, IRequestHandler<UserDeleteRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDeleteHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for handling user-related operations.</param>
        public UserDeleteHandler(DbContext db) : base(db)
        {
        }

        /// <summary>
        /// Returns a queryable collection of <see cref="User"/> entities with their associated <see cref="UserSkills"/> included.
        /// Overrides the base method to apply eager loading for the <see cref="User.UserSkills"/> navigation property,
        /// allowing related user skill data to be retrieved in the same query.
        /// </summary>
        /// <param name="isNoTracking">
        /// If <c>true</c>, disables EF Core's change tracking to improve performance in read-only scenarios.
        /// If <c>false</c>, enables tracking to allow entity updates after querying.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{User}"/> that includes related <see cref="UserSkills"/> data.
        /// </returns>
        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(u => u.UserSkills);
        }

        /// <summary>
        /// Handles the request to delete a user.
        /// </summary>
        /// <param name="request">The request containing the information for the user to be deleted.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="CommandResponse"/> indicating the success or failure of the user deletion operation.</returns>
        public async Task<CommandResponse> Handle(UserDeleteRequest request, CancellationToken cancellationToken)
        {
            // Retrieve the user from the database, including their associated user skills
            var user = await Query().SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            // Check if the user exists
            if (user is null)
                return Error("User not found!");

            // Remove the user's skills from the UserSkills table
            Delete(user.UserSkills);

            // Remove the user from the Users table
            await Delete(user, cancellationToken);

            return Success("User deleted successfully", user.Id);
        }
    }
}
