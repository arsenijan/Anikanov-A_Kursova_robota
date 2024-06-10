using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

using System;
using API.Data;
using API.Models;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tasks>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task); 
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutTask([FromForm]Tasks task)
        {
          

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                 await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.Id))
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
        public async Task<ActionResult<Task>> PostTask([FromForm] TaskDTO task)
        {
            Console.WriteLine("From Create________________________________________________");
            var TasksData = new Tasks { Grade = task.Grade, Name = task.Name };
            _context.Tasks.Add(TasksData);
           var user = _context.Users.FindAsync(task.StudentId).Result;
            user.TasksList.Add(TasksData);
            _context.Entry(user).State = EntityState.Modified;
            var discipline = _context.Disciplines.FindAsync(task.DisciplineId).Result;
            discipline.TasksList.Add(TasksData);
            _context.Entry(discipline).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisciplineId { get; set; }
     
        public int StudentId { get; set; }
        public int Grade { get; set; }
    }
}