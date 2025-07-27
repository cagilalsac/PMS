using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Users.APP.Domain;

namespace Users.APP.Features.Roles
{
    /// <summary>
    /// Represents a request to update an existing role.
    /// </summary>
    public class RoleUpdateRequest : Request, IRequest<CommandResponse>
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <remarks>
        /// The name is required and must have a maximum length of 10 characters.
        /// </remarks>
        [Required]
        [StringLength(10)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Handles the update of an existing role in the system.
    /// </summary>
    public class RoleUpdateHandler : Service<Role>, IRequestHandler<RoleUpdateRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleUpdateHandler"/> class.
        /// </summary>
        /// <param name="db">The database context used for handling role-related operations.</param>
        public RoleUpdateHandler(UsersDb db) : base(db) // DO NOT FORGET TO CHANGE THE CONSTRUCTOR'S PARAMETER from "DbContext db" to "UsersDb db"!
        {
        }

        /// <summary>
        /// Handles the request to update an existing role.
        /// </summary>
        /// <param name="request">The request containing the role ID and the updated role name.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="CommandResponse"/> indicating the success or failure of the role update operation.</returns>
        public async Task<CommandResponse> Handle(RoleUpdateRequest request, CancellationToken cancellationToken)
        {
            // Check if the updated role name already exists (excluding the current role)
            if (await Query().AnyAsync(r => r.Id != request.Id && r.Name == request.Name, cancellationToken))
                return Error("Role with the same name exists!");

            // Find the role by ID
            var role = await Query().SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (role is null)
                return Error("Role not found!");

            // Update the role's name and save changes
            role.Name = request.Name?.Trim();
            await Update(role, cancellationToken);

            return Success("Role updated successfully.", role.Id);
        }
    }
}
