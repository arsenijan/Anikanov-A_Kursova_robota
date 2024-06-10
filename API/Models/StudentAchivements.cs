using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class StudentAchievement
    {
        [Key]
        public int AchievementId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual User Student { get; set; }

        public string ParticipationInOlympiads { get; set; }

        public string CourseWorkResults { get; set; }

        public string DiplomaResults { get; set; }
    }
}
