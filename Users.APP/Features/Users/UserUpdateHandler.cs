using CORE.APP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;
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
        /// Gets or sets the role IDs of the user.
        /// </summary>
        [Required]
        public List<int> RoleIds { get; set; } = new List<int>();

        /// <summary>
        /// Gets or sets the phone of the user.
        /// </summary>
        /// <remarks>
        /// The phone is required and must be maximum 15 characters in length.
        /// </remarks>
        [Required, StringLength(15)]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the e-mail of the user.
        /// </summary>
        /// <remarks>
        /// The e-mail is required and must be maximum 200 characters in length.
        /// </remarks>
        [Required, StringLength(200)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the address of the user.
        /// </summary>
        /// <remarks>
        /// The address is optional and can be null.
        /// </remarks>
        public string Address { get; set; }
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
        /// Returns a queryable collection of <see cref="User"/> entities with their associated <see cref="UserRole"/> 
        /// and <see cref="UserDetail"/> collections included.
        /// Overrides the base method to apply eager loading for the relational data,
        /// allowing related user roles and user details data to be retrieved in the same query.
        /// </summary>
        /// <param name="isNoTracking">
        /// If <c>true</c>, disables EF Core's change tracking to improve performance in read-only scenarios.
        /// If <c>false</c>, enables tracking to allow entity updates after querying.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{User}"/> that includes related <see cref="UserRole"/> and <see cref="UserDetail"/> collections.
        /// </returns>
        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(u => u.UserRoles).Include(u => u.UserDetails);
        }

        /// <summary>
        /// Handles the request to update a user's information.
        /// </summary>
        /// <param name="request">The request containing the updated user data.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation, with a result of a <see cref="CommandResponse"/> indicating the outcome of the operation.</returns>
        public async Task<CommandResponse> Handle(UserUpdateRequest request, CancellationToken cancellationToken)
        {
            // Check if another active user with the same username exists (excluding the user being updated)
            if (await Query().AnyAsync(u => u.Id != request.Id && u.UserName == request.UserName && u.IsActive, cancellationToken))
                return Error("Active user with the same user name exists!");

            // Retrieve the user from the database
            var user = await Query().SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user is null)
                return Error("User not found!");

            // Remove existing user roles before updating
            Delete(user.UserRoles);

            // Remove existing user details before updating
            Delete(user.UserDetails);

            // Update the user information
            user.IsActive = request.IsActive;
            user.Password = request.Password;
            user.UserName = request.UserName;
            user.RoleIds = request.RoleIds;
            user.UserDetails = new List<UserDetail>()
            {
                new UserDetail()
                {
                    Address = request.Address?.Trim(), // if request.Address value is null assign null, otherwise assign trimmed request.Address value
                    Email = request.Email?.Trim(), // ? may not be used since request.Email is required and can't be null
                    Phone = request.Phone?.Trim() // ? may not be used since request.Phone is required and can't be null
                }
            };

            // Save the updated user data to the database
            await Update(user, cancellationToken);

            return Success("User updated successfully.", user.Id);
        }
    }
}
