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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
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

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                                      .Include(x => x.OlympiadParticipations).Include(x=>x.Qualifications)
                                      .Include(x => x.TasksList).Include(x=>x.Disciplines)
                                      .ToListAsync();

            var userDtos = users.Select(u => new UserDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                 AcademicDegree = u.AcademicDegree,
                  Position = u.Position,
                   Role = u.Role,
                 Qualifications = u.Qualifications?.Select(q=>new QualificationDto
                 { 
                     QualificationId = q.QualificationId,
                     Certificates = q.Certificates,
                      CourseOrSeminar = q.CourseOrSeminar,
                       Date = q.Date

                 }).ToList(),

                 Disciplines = u.Disciplines?.Select(q=>new DisciplineDto
                 {
                     DisciplineId = q.DisciplineId,
                     Name = q.Name,
                      Department = q.Department
                       
                 }).ToList(), 
                OlympiadParticipations = u.OlympiadParticipations?.Select(q => new OlympiadParticipationDto
                {
                    OlympiadId = q.OlympiadId,
                     Awards = q.Awards,
                      Date = q.Date,
                       OlympiadName = q.OlympiadName
                }).ToList(),
                TasksList = u.TasksList?.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                     Grade = t.Grade
                }).ToList()
            }).ToList();

            return Ok(userDtos);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users
                                     .Include(x => x.OlympiadParticipations)
                                     .Include(x => x.Qualifications)
                                     .Include(x => x.StudentAchievements)
                                     .Include(x => x.Disciplines).Include(x=>x.TasksList)
                                     .FirstOrDefaultAsync(x => x.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Role = user.Role,
                Qualifications = user.Qualifications.Select(q => new QualificationDto
                {
                     QualificationId = q.QualificationId,
                    Certificates = q.Certificates,
                    CourseOrSeminar = q.CourseOrSeminar,
                    Date = q.Date

                }).ToList(),

                Disciplines = user.Disciplines.Select(q => new DisciplineDto
                {
                     DisciplineId = q .DisciplineId,
                    Name = q.Name,
                    Department = q.Department

                }).ToList(),
                OlympiadParticipations = user.OlympiadParticipations.Select(q => new OlympiadParticipationDto
                {
                    OlympiadId = q.OlympiadId,
                    Awards = q.Awards,
                    Date = q.Date,
                    OlympiadName = q.OlympiadName
                }).ToList(),
                TasksList = user.TasksList.Select(q => new TaskDto
                {
                     Id = q.Id,
                      Name = q.Name,
                       Grade = q.Grade
                       
                }).ToList()
            };

            return Ok(userDto);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser( [FromForm] UserDto model)
        {

            Console.WriteLine("____________________________________________________________________________FROM UPDATE______________________________________________________________________");
            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.Role = model.Role;
            user.Password = model.Password;
            user.Position = model.Position;
            user.AcademicDegree = model.AcademicDegree;

            var qualification = user.Qualifications.FirstOrDefault();
            if (qualification != null)
            {
                qualification.CourseOrSeminar = model.CourseOrSeminar;
                qualification.Date = model.Date;
                qualification.Certificates = model.Certificates;
            }
            else
            {
                user.Qualifications.Add(new Qualification
                {
                    CourseOrSeminar = model.CourseOrSeminar,
                    Date = model.Date,
                    Certificates = model.Certificates
                });
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        [HttpPost("create")]
        public async Task<ActionResult<User>> PostUser([FromForm]User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await LogAction($"DeleteUser {id}");

            return NoContent();
        }


    }
    public class UserDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
        public string? Position { get; set; }
        public string? AcademicDegree { get; set; }
        public string? CourseOrSeminar { get; set; }
        public DateTime Date { get; set; }
        public string? Certificates { get; set; }
        public List<QualificationDto>? Qualifications { get; set; }
        public List<TaskDto>? TasksList { get; set; }
        public List<OlympiadParticipationDto>? OlympiadParticipations { get; set; }
        public List<DisciplineDto>? Disciplines { get; set; }
    }
   
    public class QualificationDto
    {
        public int? QualificationId { get; set; }
        public string? CourseOrSeminar { get; set; }

        public DateTime Date { get; set; }

        public string? Certificates { get; set; }
    }
    public class OlympiadParticipationDto
    {
        public int? OlympiadId { get; set; }
        public string? OlympiadName { get; set; }

        public DateTime Date { get; set; }

        public string? Awards { get; set; }
    }
}