using CORE.APP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;
using Users.APP.Services;

namespace Users.APP.Features.Users
{
    /// <summary>
    /// Represents a request to create a new user.
    /// </summary>
    public class UserCreateRequest : Request, IRequest<CommandResponse>
    {
        /// <summary>
        /// Gets or sets the username for the new user.
        /// </summary>
        /// <remarks>
        /// The username is required and must be between 3 and 30 characters in length.
        /// </remarks>
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password for the new user.
        /// </summary>
        /// <remarks>
        /// The password is required and must be between 3 and 15 characters in length.
        /// </remarks>
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets whether the user is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the role IDs for the user.
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
    /// Handles the creation of a new user in the system.
    /// </summary>
    public class UserCreateHandler : UserService, IRequestHandler<UserCreateRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserCreateHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for handling user-related operations.</param>
        public UserCreateHandler(DbContext db) : base(db)
        {
        }

        /// <summary>
        /// Handles the request to create a new user.
        /// </summary>
        /// <param name="request">The request containing the user creation information.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="CommandResponse"/> indicating the success or failure of the user creation operation.</returns>
        public async Task<CommandResponse> Handle(UserCreateRequest request, CancellationToken cancellationToken)
        {
            // Check if an active user with the same username already exists
            if (await Query().AnyAsync(u => u.UserName == request.UserName && u.IsActive, cancellationToken))
                return Error("Active user with the same user name exists!");

            // Create a new user entity with user details from the request data
            var user = new User()
            {
                IsActive = request.IsActive,
                Password = request.Password,
                RoleIds = request.RoleIds,
                UserName = request.UserName,
                UserDetails = new List<UserDetail>()
                {
                    new UserDetail()
                    {
                        Address = request.Address?.Trim(), // if request.Address value is null assign null, otherwise assign trimmed request.Address value
                        Email = request.Email?.Trim(), // ? may not be used since request.Email is required and can't be null
                        Phone = request.Phone?.Trim() // ? may not be used since request.Phone is required and can't be null
                    }
                }
            };

            // Add the new user with user details to the database
            await Create(user, cancellationToken);

            return Success("User created successfully.", user.Id);
        }
    }
}
