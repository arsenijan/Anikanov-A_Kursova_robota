using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Models;
using API.Data;
namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisciplinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DisciplinesController(AppDbContext context)
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
        public async Task<ActionResult<IEnumerable<DisciplineDto>>> GetDisciplines()
        {
            var disciplines = await _context.Disciplines
                                            .Include(x => x.TasksList)
                                            .Include(x => x.Users)
                                            .ToListAsync();

            var disciplineDtos = disciplines.Select(d => new DisciplineDto
            {
                DisciplineId = d.DisciplineId,
                Name = d.Name,
                Department = d.Department,
                Source = d.Source,

                TasksList = d.TasksList.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Grade = t.Grade,
                }).ToList(),
                Users = d.Users.Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Role = u.Role,
                    TasksList = u.TasksList.Select(t => new TaskDto
                    {
                        Grade = t.Grade,
                        Id = t.Id,
                        Name = t.Name,
                    }).ToList()
                }).ToList()
            }).ToList();

            return Ok(disciplineDtos);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Disciplines>> GetDiscipline(int id)
        {
            var discipline = await _context.Disciplines.FindAsync(id);

            if (discipline == null)
            {
                return NotFound();
            }


            return discipline;
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutDiscipline([FromForm] DisciplineDto discipline)
        {

            if (!discipline.TeacherId.Equals(null))
            {
                var user = await _context.Users.Include(x => x.Disciplines).FirstOrDefaultAsync(x => x.UserId == discipline.TeacherId);
                if (user.Role != "Викладач")
                {
                    return BadRequest();
                }
                var Discipline = await _context.Disciplines.Include(x => x.Users).FirstOrDefaultAsync(x => x.DisciplineId == discipline.DisciplineId);
                if (user.Disciplines.Contains(Discipline))
                {
                    return BadRequest();
                }
                user.Disciplines.Add(Discipline);
                _context.Entry(user).State = EntityState.Modified;
                _context.Entry(Discipline).State = EntityState.Modified;
                Console.WriteLine(user.Disciplines.First().Name);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisciplineExists(discipline.DisciplineId))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }
            else if (!discipline.StudentId.Equals(null))
            {
                var user = await _context.Users.Include(x => x.Disciplines).FirstOrDefaultAsync(x => x.UserId == discipline.StudentId);
                if (user.Role != "Студент")
                {
                    return BadRequest();
                }
                var Discipline = await _context.Disciplines.Include(x => x.Users).FirstOrDefaultAsync(x => x.DisciplineId == discipline.DisciplineId);
                if (user.Disciplines.Contains(Discipline))
                {
                    return BadRequest();
                }
                user.Disciplines.Add(Discipline);
                _context.Entry(user).State = EntityState.Modified;
                _context.Entry(Discipline).State = EntityState.Modified;
                Console.WriteLine(user.Disciplines.First().Name);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisciplineExists(discipline.DisciplineId))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }
            else
            {
                var existingDiscipline = await _context.Disciplines.FindAsync(discipline.DisciplineId);
                if (existingDiscipline == null)
                {
                    return NotFound();
                }

                existingDiscipline.Name = discipline.Name;
                existingDiscipline.Tasks = discipline.Tasks;
                existingDiscipline.Department = discipline.Department;


                if (discipline.Image != null && discipline.Image.Length > 0)
                {

                    var fileName = Path.GetFileNameWithoutExtension(discipline.Image.FileName);
                    var extension = Path.GetExtension(discipline.Image.FileName);
                    var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await discipline.Image.CopyToAsync(stream);
                    }
                    var serverUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                    var imagePath = $"{serverUrl}/images/{newFileName}";
                    existingDiscipline.Source = imagePath;
                }


                _context.Entry(existingDiscipline).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisciplineExists(discipline.DisciplineId))
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
        }


        [HttpPost("create")]
        public async Task<ActionResult<Disciplines>> PostDiscipline([FromForm] DisciplineDto discipline)
        {

            string imagePath = null;
            if (discipline.Image != null)
            {

                var fileName = Path.GetFileNameWithoutExtension(discipline.Image.FileName);
                var extension = Path.GetExtension(discipline.Image.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";


                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", newFileName);


                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await discipline.Image.CopyToAsync(stream);
                }

                var serverUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                imagePath = $"{serverUrl}/images/{newFileName}";
            }
            _context.Disciplines.Add(new Disciplines { Department = discipline.Department, Name = discipline.Name, Tasks = discipline.Tasks, Source = imagePath });
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDiscipline), new { id = discipline.DisciplineId }, discipline);
        }

        [HttpDelete("delete/{id}")]

        public async Task<IActionResult> DeleteDiscipline(int id)
        {
            var discipline = await _context.Disciplines.FindAsync(id);
            if (discipline == null)
            {
                return NotFound();
            }

            _context.Disciplines.Remove(discipline);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool DisciplineExists(int id)
        {
            return _context.Disciplines.Any(e => e.DisciplineId == id);
        }
    }

    public class DisciplineDto
    {
        public int DisciplineId { get; set; }
        public string? Name { get; set; }
        public string? Tasks { get; set; }
        public int? TeacherId { get; set; }
        public int? StudentId { get; set; }
        public string? Department { get; set; }
        public IFormFile? Image { get; set; }
        public List<TaskDto>? TasksList { get; set; }
        public List<UserDto>? Users { get; set; }
        public string? Source { get; set; }
    }

    public class TaskDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? Grade { get; set; }
    }



}