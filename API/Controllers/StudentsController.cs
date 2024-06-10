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
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
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
        public async Task<ActionResult<IEnumerable<User>>> GetStudents()
        {
            await LogAction("GetStudents");
            return await _context.Users.Where(u => u.Role == "Student").ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetStudent(int id)
        {
            var student = await _context.Users.FindAsync(id);

            if (student == null || student.Role != "Student")
            {
                return NotFound();
            }

            await LogAction($"GetStudent {id}");
            return student;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateStudent(User student)
        {
            if (student == null || student.Role != "Student")
            {
                return BadRequest();
            }

            _context.Users.Add(student);
            await _context.SaveChangesAsync();

            await LogAction($"CreateStudent {student.UserId}");
            return CreatedAtAction(nameof(GetStudent), new { id = student.UserId }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, User student)
        {
            if (id != student.UserId || student.Role != "Student")
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await LogAction($"UpdateStudent {id}");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Users.FindAsync(id);
            if (student == null || student.Role != "Student")
            {
                return NotFound();
            }

            _context.Users.Remove(student);
            await _context.SaveChangesAsync();

            await LogAction($"DeleteStudent {id}");
            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id && e.Role == "Student");
        }
    }
}