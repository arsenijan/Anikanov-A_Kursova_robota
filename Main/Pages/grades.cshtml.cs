using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;

namespace Main.Pages
{
    public class gradesModel : PageModel
    {
        public List<Disciplines>? Disciplines { get; set; }
        [BindProperty]
        public Tasks Task { get; set; }
        [BindProperty]
        public int DisciplineId { get; set; }
        [BindProperty]
        public int StudentId { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var apiUrlGetAll = "https://localhost:7149/api/Disciplines/get";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrlGetAll);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Disciplines = JsonConvert.DeserializeObject<List<Disciplines>>(jsonResponse);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return Redirect("/authorize");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    return Page();
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var apiUrlPost = "https://localhost:7149/api/Tasks/create";
            using (var client = new HttpClient())
            {
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(Task.Name), "Name");
                form.Add(new StringContent(Task.Grade.ToString()), "grade");
                form.Add(new StringContent(DisciplineId.ToString()), "disciplineId");
                form.Add(new StringContent(StudentId.ToString()), "studentId");
                var response = await client.PostAsync(apiUrlPost, form);
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("/grades"); 
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return Redirect("/authorize");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    return Page();
                }
            }
        }
    }

}
