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
    public class TeachersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeachersController(AppDbContext context)
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
        public async Task<ActionResult<IEnumerable<User>>> GetTeachers()
        {
            await LogAction("GetTeachers");
            return await _context.Users.Where(u => u.Role == "Teacher").ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetTeacher(int id)
        {
            var teacher = await _context.Users.FindAsync(id);

            if (teacher == null || teacher.Role != "Teacher")
            {
                return NotFound();
            }

            await LogAction($"GetTeacher {id}");
            return teacher;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateTeacher(User teacher)
        {
            if (teacher == null || teacher.Role != "Teacher")
            {
                return BadRequest();
            }

            _context.Users.Add(teacher);
            await _context.SaveChangesAsync();

            await LogAction($"CreateTeacher {teacher.UserId}");
            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.UserId }, teacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, User teacher)
        {
            if (id != teacher.UserId || teacher.Role != "Teacher")
            {
                return BadRequest();
            }

            _context.Entry(teacher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await LogAction($"UpdateTeacher {id}");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Users.FindAsync(id);
            if (teacher == null || teacher.Role != "Teacher")
            {
                return NotFound();
            }

            _context.Users.Remove(teacher);
            await _context.SaveChangesAsync();

            await LogAction($"DeleteTeacher {id}");
            return NoContent();
        }

        private bool TeacherExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id && e.Role == "Teacher");
        }
    }
}