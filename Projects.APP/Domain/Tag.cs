using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projects.APP.Domain
{
    /// <summary>
    /// Represents a tag, inheriting from <see cref="Entity"/>.
    /// </summary>
    public class Tag : Entity
    {
        /// <summary>
        /// Gets or sets the name of the tag.
        /// The name is required and has a maximum length of 150 characters.
        /// </summary>
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of relational project tags.
        /// This is a navigational property representing the projects associated with the tag for project-tag many to many relationship.
        /// </summary>
        public List<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();

        /// <summary>
        /// Gets or sets the list of project IDs associated with the tag by returning relational project IDs 
        /// from the relational ProjectTags collection, or setting the relational ProjectTags collection 
        /// by assigning from the project IDs (value) set to this property.
        /// This property is not mapped to the database therefore there won't be a column in the table.
        /// </summary>
        [NotMapped]
        public List<int> ProjectIds
        {
            get => ProjectTags.Select(projectTag => projectTag.ProjectId).ToList();
            set => ProjectTags = value.Select(projectId => new ProjectTag() { ProjectId = projectId }).ToList();
        }
    }
}
