using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Role { get; set; }
        [Required]
        public string Password { get; set; }

        public string? Position { get; set; }

        public string? AcademicDegree { get; set; }

        public virtual ICollection<Tasks>? TasksList { get; set; } = new List<Tasks>();

        public virtual ICollection<Disciplines> Disciplines { get; set; } = new List<Disciplines>();

        public virtual ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();

        public virtual ICollection<Qualification> Qualifications { get; set; } = new List<Qualification>();

        public virtual ICollection<OlympiadParticipation> OlympiadParticipations { get; set; } = new List<OlympiadParticipation>();
    }
}
