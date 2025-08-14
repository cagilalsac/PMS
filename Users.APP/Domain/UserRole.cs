using CORE.APP.Domain;

namespace Users.APP.Domain
{
    /// <summary>
    /// Represents the association between a user and a role, enabling a many-to-many relationship.
    /// </summary>
    public class UserRole : Entity
    {
        /// <summary>
        /// Gets or sets the foreign key for the associated user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this user-role relationship.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the associated role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role associated with this user-role relationship.
        /// </summary>
        public Role Role { get; set; }
    }
}
