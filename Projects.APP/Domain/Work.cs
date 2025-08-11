using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Projects.APP.Domain
{
    /// <summary>
    /// Represents a work item associated with a project, including details such as name, description, dates, 
    /// and project-work one to many relationship properties.
    /// Inherits from the base class Entity.
    /// </summary>
    public class Work : Entity
    {
        /// <summary>
        /// Gets or sets the name of the work item.
        /// This field is required and must be a maximum of 300 characters long.
        /// </summary>
        [Required, StringLength(300)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the work item.
        /// This field is optional.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the start date of the work item.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the due date of the work item.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the project ID associated with this work item.
        /// This field is a foreign key linking to the Project entity for project-work one to many relationship and can be null.
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the project associated with this work item for project-work one to many relationship.
        /// This is a navigational property.
        /// </summary>
        public Project Project { get; set; }
    }
}
