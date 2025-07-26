using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Projects.APP.Domain
{
    public class Work : Entity
    {
        [Required, StringLength(300)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DueDate { get; set; }

        public int? ProjectId { get; set; }

        public Project Project { get; set; }
    }
}
