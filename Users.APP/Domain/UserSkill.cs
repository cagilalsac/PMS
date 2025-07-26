using CORE.APP.Domain;

namespace Users.APP.Domain
{
    /// <summary>
    /// Represents the relationship between a user and a skill.
    /// </summary>
    public class UserSkill : Entity
    {
        /// <summary>
        /// Gets or sets the user ID associated with the skill.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the skill ID associated with the user.
        /// </summary>
        public int SkillId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this relationship.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the skill associated with this relationship.
        /// </summary>
        public Skill Skill { get; set; }
    }
}
