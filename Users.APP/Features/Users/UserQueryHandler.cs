using CORE.APP.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;
using Users.APP.Features.Roles;
using Users.APP.Services;

namespace Users.APP.Features.Users
{
    /// <summary>
    /// Represents a request to query users.
    /// </summary>
    public class UserQueryRequest : Request, IRequest<IQueryable<UserQueryResponse>>
    {
    }

    /// <summary>
    /// Represents the response containing user data for a query.
    /// </summary>
    public class UserQueryResponse : Response
    {
        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the status indicating if the user is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the formatted status of the user's activity (Active/Inactive).
        /// </summary>
        public string IsActiveF { get; set; }

        /// <summary>
        /// Gets or sets the IDs of the user's roles.
        /// </summary>
        public List<int> RoleIds { get; set; } = new List<int>();

        /// <summary>
        /// Gets or sets the list of roles associated with the user.
        /// </summary>
        public List<RoleQueryResponse> Roles { get; set; }

        /// <summary>
        /// Gets or sets the phone of the user.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the e-mail of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the address of the user.
        /// </summary>
        public string Address { get; set; }
    }

    /// <summary>
    /// Handles the request to query users.
    /// </summary>
    public class UserQueryHandler : UserService, IRequestHandler<UserQueryRequest, IQueryable<UserQueryResponse>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserQueryHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for handling user-related operations.</param>
        public UserQueryHandler(DbContext db) : base(db)
        {
        }

        /// <summary>
        /// Returns a queryable collection of <see cref="User"/> entities with related data included.
        /// Overrides the base method to eagerly load the associated <see cref="UserRole"/>, 
        /// <see cref="Role"/> and <see cref="UserDetail"/> navigation properties. 
        /// The results are ordered by the user's <see cref="User.UserName"/> property.
        /// </summary>
        /// <param name="isNoTracking">
        /// If <c>true</c>, disables change tracking for better performance in read-only queries.
        /// If <c>false</c>, enables tracking to allow modifications to queried entities.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{User}"/> with <see cref="UserRole"/>, <see cref="Role"/> 
        /// and <see cref="UserDetail"/> included, ordered by user name.
        /// </returns>
        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking)
                       .Include(u => u.UserRoles)
                       .ThenInclude(ur => ur.Role)
                       .Include(u => u.UserDetails)
                       .OrderBy(u => u.UserName);
        }

        /// <summary>
        /// Handles the request to query users.
        /// </summary>
        /// <param name="request">The request containing the user data query parameters.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation, with a result of a queryable <see cref="UserQueryResponse"/>.</returns>
        public Task<IQueryable<UserQueryResponse>> Handle(UserQueryRequest request, CancellationToken cancellationToken)
        {
            var query = Query().Select(u => new UserQueryResponse()
            {
                Id = u.Id,
                IsActive = u.IsActive,
                IsActiveF = u.IsActive ? "Active" : "Inactive",
                Password = u.Password,
                UserName = u.UserName,
                RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList(),
                Roles = u.UserRoles.Select(ur => new RoleQueryResponse()
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name
                }).ToList(),
                Phone = u.UserDetails.FirstOrDefault() == null ? string.Empty : u.UserDetails.FirstOrDefault().Phone,
                Email = u.UserDetails.FirstOrDefault() == null ? string.Empty : u.UserDetails.FirstOrDefault().Email,
                Address = u.UserDetails.FirstOrDefault() == null ? string.Empty : u.UserDetails.FirstOrDefault().Address
            });
            return Task.FromResult(query);
        }
    }
}
