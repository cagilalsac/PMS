using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Users.APP.Domain
{
    /// <summary>
    /// Represents detailed information associated with a user.
    /// </summary>
    public class UserDetail : Entity
    {
        /// <summary>
        /// Gets or sets the foreign key for the associated user for user-userdetail one to many relationship.
        /// However, we will have only one user detail data associated with a user in our application providing one to one relationship.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user to whom these details belong.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// This field is required and must be a maximum of 15 characters.
        /// </summary>
        [Required, StringLength(15)]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// This field is required and must be a maximum of 200 characters.
        /// </summary>
        [Required, StringLength(200)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the address of the user.
        /// This field is optional.
        /// </summary>
        public string Address { get; set; }
    }
}
