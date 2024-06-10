using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using API.Models;
using API.Data;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class StudentOlympiadsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentOlympiadsController(AppDbContext context)
        {
            _context = context;
        }

        private async System.Threading.Tasks.Task LogAction(string action)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var log = new Log
            {
                Timestamp = DateTime.UtcNow,
                Action = action,
                UserId = userId
            };
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OlympiadParticipation>>> GetStudentOlympiads()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Student")
            {

                return await _context.OlympiadParticipations.Where(so => so.OlympiadId == userId).ToListAsync();
            }

            return await _context.OlympiadParticipations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OlympiadParticipation>> GetStudentOlympiad(int id)
        {
            var olympiad = await _context.OlympiadParticipations.FindAsync(id);

            if (olympiad == null)
            {
                return NotFound();
            }

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Student" && olympiad.OlympiadId != userId)
            {
                return Forbid();
            }

            return olympiad;
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutStudentOlympiad([FromForm]OlympiadParticipation olympiad)
        {
            Console.WriteLine("FROM UPDATE_____________________________________________________");
            var existingOlympiad = await _context.OlympiadParticipations.FindAsync(olympiad.OlympiadId);
            if (existingOlympiad == null)
            {
                return NotFound();
            }

            existingOlympiad.OlympiadName = olympiad.OlympiadName;
            existingOlympiad.Awards = olympiad.Awards;
            existingOlympiad.Date = olympiad.Date;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentOlympiadExists(olympiad.OlympiadId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("create")]
        public async Task<ActionResult<OlympiadParticipation>> PostStudentOlympiad([FromForm] OlympiadDTO olympiad)
        {
            var Olympiad = new OlympiadParticipation { Awards = olympiad.Awards, OlympiadName = olympiad.OlympiadName, Date = olympiad.Date };
            _context.OlympiadParticipations.Add(Olympiad);
           var user = _context.Users.FindAsync(olympiad.StudentId).Result;
            user.OlympiadParticipations.Add(Olympiad);
            await _context.SaveChangesAsync();
            var user2 = _context.Users.FindAsync(olympiad.StudentId).Result;
            return CreatedAtAction(nameof(GetStudentOlympiad), new { id = olympiad.OlympiadId }, olympiad);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStudentOlympiad(int id)
        {
            var olympiad = await _context.OlympiadParticipations.FindAsync(id);
            if (olympiad == null)
            {
                return NotFound();
            }

            _context.OlympiadParticipations.Remove(olympiad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentOlympiadExists(int id)
        {
            return _context.OlympiadParticipations.Any(e => e.OlympiadId == id);
        }
    }
   public class OlympiadDTO
    {
        public int OlympiadId { get; set; }

        public int StudentId { get; set; }
        public string OlympiadName { get; set; }

        public DateTime Date { get; set; }

        public string Awards { get; set; }
    }

}