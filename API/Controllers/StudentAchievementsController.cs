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

    public class StudentAchievementsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentAchievementsController(AppDbContext context)
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
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentAchievement>>> GetStudentAchievements()
        {
            await LogAction("GetStudentAchievements");
            return await _context.StudentAchievements.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentAchievement>> GetStudentAchievement(int id)
        {
            var achievement = await _context.StudentAchievements.FindAsync(id);

            if (achievement == null)
            {
                return NotFound();
            }

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Student" && achievement.StudentId != userId)
            {
                return Forbid();
            }

            await LogAction($"GetStudentAchievement {id}");
            return achievement;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentAchievement(int id, StudentAchievement achievement)
        {
            if (id != achievement.StudentId)
            {
                return BadRequest();
            }

            _context.Entry(achievement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await LogAction($"PutStudentAchievement {id}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentAchievementExists(id))
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

        [HttpPost]
        public async Task<ActionResult<StudentAchievement>> PostStudentAchievement(StudentAchievement achievement)
        {
            _context.StudentAchievements.Add(achievement);
            await _context.SaveChangesAsync();
            await LogAction($"PostStudentAchievement {achievement.StudentId}");

            return CreatedAtAction(nameof(GetStudentAchievement), new { id = achievement.StudentId }, achievement);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentAchievement(int id)
        {
            var achievement = await _context.StudentAchievements.FindAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }

            _context.StudentAchievements.Remove(achievement);
            await _context.SaveChangesAsync();
            await LogAction($"DeleteStudentAchievement {id}");

            return NoContent();
        }

        private bool StudentAchievementExists(int id)
        {
            return _context.StudentAchievements.Any(e => e.StudentId == id);
        }
    }
}