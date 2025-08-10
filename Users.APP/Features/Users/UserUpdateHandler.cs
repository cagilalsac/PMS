using Users.APP.Domain;
using CORE.APP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Services;

namespace Users.APP.Features.Users
{
    /// <summary>
    /// Represents a request to update a user's information.
    /// </summary>
    public class UserUpdateRequest : Request, IRequest<CommandResponse>
    {
        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the status indicating if the user is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the surname of the user.
        /// </summary>
        [StringLength(50)]
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the registration date of the user.
        /// </summary>
        public DateTime? RegistrationDate { get; set; }

        /// <summary>
        /// Gets or sets the role ID of the user.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the list of skill IDs associated with the user.
        /// </summary>
        public List<int> SkillIds { get; set; }
    }

    /// <summary>
    /// Handles the request to update a user's information.
    /// </summary>
    public class UserUpdateHandler : UserService, IRequestHandler<UserUpdateRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpdateHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for handling user-related operations.</param>
        public UserUpdateHandler(DbContext db) : base(db)
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
        /// Handles the request to update a user's information.
        /// </summary>
        /// <param name="request">The request containing the updated user data.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation, with a result of a <see cref="CommandResponse"/> indicating the outcome of the operation.</returns>
        public async Task<CommandResponse> Handle(UserUpdateRequest request, CancellationToken cancellationToken)
        {
            // Check if another user with the same username or full name exists (excluding the user being updated)
            if (await Query().AnyAsync(u => u.Id != request.Id && 
                                           (u.UserName == request.UserName || (u.Name == request.Name && u.Surname == request.Surname)), 
                                           cancellationToken))
                return Error("User with the same user name or full name exists!");

            // Retrieve the user from the database
            var user = await Query().SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user is null)
                return Error("User not found!");

            // Remove existing user skills before updating
            Delete(user.UserSkills);

            // Update the user information
            user.IsActive = request.IsActive;
            user.Name = request.Name?.Trim();
            user.Password = request.Password;
            user.RoleId = request.RoleId;
            user.Surname = request.Surname?.Trim();
            user.UserName = request.UserName;
            user.RegistrationDate = request.RegistrationDate;
            user.SkillIds = request.SkillIds;

            // Save the updated user data to the database
            await Update(user, cancellationToken);

            return Success("User updated successfully.", user.Id);
        }
    }
}
