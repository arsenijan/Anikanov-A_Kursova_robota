using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class OlympiadParticipation
    {
        [Key]
        public int OlympiadId { get; set; }

      
        public string OlympiadName { get; set; }

        public DateTime Date { get; set; }

        public string Awards { get; set; }
    }
}