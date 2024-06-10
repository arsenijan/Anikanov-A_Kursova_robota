using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Disciplines
    {
        [Key]
        public int DisciplineId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Source { get; set; }
        public string Department { get; set; }
        public string Tasks { get; set; }
        public virtual ICollection<Tasks>? TasksList { get; set; } = new List<Tasks>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
  
    }
}
