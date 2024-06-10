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
    public class TeacherQualificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeacherQualificationsController(AppDbContext context)
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
        public async Task<ActionResult<IEnumerable<Qualification>>> GetTeacherQualifications()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Teacher")
            {
                await LogAction($"GetTeacherQualifications by Teacher {userId}");
                return await _context.Qualifications.Where(tq => tq.TeacherId == userId).ToListAsync();
            }

            await LogAction("GetTeacherQualifications");
            return await _context.Qualifications.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Qualification>> GetTeacherQualification(int id)
        {
            var qualification = await _context.Qualifications.FindAsync(id);

            if (qualification == null)
            {
                return NotFound();
            }

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Teacher" && qualification.TeacherId != userId)
            {
                return Forbid();
            }

            await LogAction($"GetTeacherQualification {id}");
            return qualification;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacherQualification(int id, Qualification qualification)
        {
            if (id != qualification.TeacherId)
            {
                return BadRequest();
            }

            _context.Entry(qualification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await LogAction($"PutTeacherQualification {id}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherQualificationExists(id))
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
        public async Task<ActionResult<Qualification>> PostTeacherQualification(Qualification qualification)
        {
            _context.Qualifications.Add(qualification);
            await _context.SaveChangesAsync();
            await LogAction($"PostTeacherQualification {qualification.TeacherId}");

            return CreatedAtAction(nameof(GetTeacherQualification), new { id = qualification.TeacherId }, qualification);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTeacherQualification(int id)
        {
            var qualification = await _context.Qualifications.FindAsync(id);
            if (qualification == null)
            {
                return NotFound();
            }

            _context.Qualifications.Remove(qualification);
            await _context.SaveChangesAsync();
        

            return NoContent();
        }

        private bool TeacherQualificationExists(int id)
        {
            return _context.Qualifications.Any(e => e.TeacherId == id);
        }
    }
}