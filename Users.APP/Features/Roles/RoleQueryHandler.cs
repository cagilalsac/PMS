using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Users.APP.Domain;
using Users.APP.Features.Users;

namespace Users.APP.Features.Roles
{
    /// <summary>
    /// Represents a request to query roles from the system.
    /// </summary>
    public class RoleQueryRequest : Request, IRequest<IQueryable<RoleQueryResponse>>
    {
        // No additional properties are required for this request.
    }

    /// <summary>
    /// Represents the response format for querying roles.
    /// </summary>
    public class RoleQueryResponse : QueryResponse
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of users associated with the role.
        /// </summary>
        /// <remarks>
        /// This property is ignored during serialization as it's intended for internal processing.
        /// </remarks>
        [JsonIgnore]
        public List<UserQueryResponse> Users { get; set; }
    }

    /// <summary>
    /// Handles the query for roles in the system.
    /// </summary>
    public class RoleQueryHandler : Service<Role>, IRequestHandler<RoleQueryRequest, IQueryable<RoleQueryResponse>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleQueryHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for role-related operations.</param>
        public RoleQueryHandler(UsersDb db) : base(db)
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
            return base.Query(isNoTracking).Include(r => r.Users).OrderBy(r => r.Name);
        }

        /// <summary>
        /// Handles the request to query roles from the database.
        /// </summary>
        /// <param name="request">The request containing the query parameters.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation, with an <see cref="IQueryable{RoleQueryResponse}"/> as the result.</returns>
        public Task<IQueryable<RoleQueryResponse>> Handle(RoleQueryRequest request, CancellationToken cancellationToken)
        {
            // Build the query to get roles along with their associated users
            var query = Query().Select(r => new RoleQueryResponse()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Users = r.Users.Select(u => new UserQueryResponse()
                    {
                        FullName = u.Name + " " + u.Surname,
                        Id = u.Id,
                        IsActive = u.IsActive,
                        IsActiveF = u.IsActive ? "Active" : "Inactive",
                        Name = u.Name,
                        Password = u.Password,
                        Surname = u.Surname,
                        UserName = u.UserName
                    }).ToList()
                });

            return Task.FromResult(query);
        }
    }
}
