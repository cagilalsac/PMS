using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Users.APP.Domain
{
    /// <summary>
    /// Represents a skill that can be associated with users.
    /// </summary>
    public class Skill : Entity
    {
        /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        /// <remarks>
        /// The name is required and has a maximum length of 125 characters.
        /// </remarks>
        [Required, StringLength(125)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of user-skill many to many relationships.
        /// </summary>
        public List<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

        /// <summary>
        /// Gets or sets a list of user IDs associated with the skill.
        /// </summary>
        /// <remarks>
        /// This property is not mapped to the database and is used for easier manipulation of UserSkills.
        /// </remarks>
        [NotMapped]
        public List<int> UserIds
        {
            get => UserSkills.Select(userSkill => userSkill.UserId).ToList();
            set => UserSkills = value.Select(userId => new UserSkill() { UserId = userId }).ToList();
        }
    }
}
