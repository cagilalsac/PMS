using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

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
        /// Gets or sets the list of user-skill relationships.
        /// </summary>
        public List<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }
}
