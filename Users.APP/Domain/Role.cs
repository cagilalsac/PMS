using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

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
        /// Gets or sets the list of users associated with this role.
        /// </summary>
        public List<User> Users { get; set; } = new List<User>();
    }
}
