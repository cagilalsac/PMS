using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Users.APP.Domain
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : Entity
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        /// <remarks>
        /// The username is required and must be between 3 and 30 characters long.
        /// </remarks>
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        /// <remarks>
        /// The password is required and must be between 3 and 15 characters long.
        /// </remarks>
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the user details information associated with the user for user-userdetail one to many relationship.
        /// However, we will have only one user detail data associated with a user in our application providing one to one relationship.
        /// </summary>
        public List<UserDetail> UserDetails { get; set; } = new List<UserDetail>();

        /// <summary>
        /// Gets or sets the list of user roles for user-role many to many relationship.
        /// </summary>
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Gets or sets a list of role IDs associated with the user.
        /// </summary>
        /// <remarks>
        /// This property is not mapped to the database and is used for easier manipulation of UserRoles.
        /// </remarks>
        [NotMapped]
        public List<int> RoleIds
        {
            get => UserRoles.Select(userRole => userRole.RoleId).ToList();
            set => UserRoles = value.Select(roleId => new UserRole() { RoleId = roleId }).ToList();
        }

        /// <summary>
        /// Gets or sets the refresh token assigned to the user.
        /// This token is used to obtain a new access token without requiring re-authentication.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time of the refresh token.
        /// This value determines when the refresh token becomes invalid. A null value implies no expiration is set.
        /// </summary>
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
