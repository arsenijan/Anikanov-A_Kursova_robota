using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;

namespace Main.Pages
{
    public class student_gradesModel : PageModel
    {
        [BindProperty]
        public User? Student { get; set; }
        [BindProperty]
        public OlympiadParticipation Olympiad { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var apiUrlGetById = $"https://localhost:7149/api/Users/get/{id}";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrlGetById);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Student = JsonConvert.DeserializeObject<User>(jsonResponse);
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

            var apiUrlPost = "https://localhost:7149/api/StudentOlympiads/create";
            using (var client = new HttpClient())
            {


                var form = new MultipartFormDataContent();

                form.Add(new StringContent(Olympiad.OlympiadName), "olympiadName");
                form.Add(new StringContent(Olympiad.Awards), "awards");
                form.Add(new StringContent(Olympiad.Date.ToShortDateString()), "date");
                form.Add(new StringContent(Student.UserId.ToString()),"studentId");
             

                var response = await client.PostAsync(apiUrlPost, form);
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("/disciplines");
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
    

