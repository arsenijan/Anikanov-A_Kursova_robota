using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Qualification
    {
        [Key]
        public int QualificationId { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; }

        public string CourseOrSeminar { get; set; }

        public DateTime Date { get; set; }

        public string Certificates { get; set; }
    }
}
