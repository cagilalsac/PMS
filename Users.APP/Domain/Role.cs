using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Users.APP.Domain
{
    /// <summary>
    /// Represents a role within the system.
    /// </summary>
    public class Role : Entity
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <remarks>
        /// The name is required and has a maximum length of 10 characters.
        /// </remarks>
        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of user roles associated with this role for user-role many to many relationship.
        /// </summary>
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Gets or sets the list of user IDs associated with this role.
        /// Setting this property updates the <see cref="UserRoles"/> collection accordingly.
        /// There is no column for this property in the Roles table since NotMapped is defined.
        /// </summary>
        [NotMapped]
        public List<int> UserIds
        {
            get => UserRoles.Select(userRole => userRole.UserId).ToList();
            set => UserRoles = value.Select(userId => new UserRole() { UserId = userId }).ToList();
        }
    }
}
