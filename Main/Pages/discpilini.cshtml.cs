using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Main.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;
using API.Models;

namespace Main.Pages
{
    
    public class DisciplinesModel : PageModel
    {
        [BindProperty]
        public DisciplineModel Discipline { get; set; }
        public List<Disciplines>? Disciplines { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
 
            var apiUrlPost = "https://localhost:7149/api/Disciplines/create";
            using (var client = new HttpClient())
            {

             
                var form = new MultipartFormDataContent();

                form.Add(new StringContent(Discipline.Name), "name");
                form.Add(new StringContent(Discipline.Tasks.ToString()), "tasks");
                form.Add(new StringContent(Discipline.Department.ToString()), "department");

                if (Discipline.Image != null)
                {
                    var stream = new MemoryStream();
                    await Discipline.Image.CopyToAsync(stream);
                    stream.Position = 0;

                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(Discipline.Image.ContentType);

                    form.Add(fileContent, "image", Discipline.Image.FileName);
                }

                var response = await client.PostAsync(apiUrlPost, form);
                Console.WriteLine(response.StatusCode);
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
        public async Task<IActionResult> OnGetAsync()
        {
            var apiUrlGetAll  = "https://localhost:7149/api/Disciplines/get";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrlGetAll);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Disciplines = JsonConvert.DeserializeObject<List<Disciplines>>(jsonResponse);
                    foreach(var dis in Disciplines)
                    {
                        foreach(var us in dis.Users)
                        {
                            Console.WriteLine(us.Role);
                        }
                    }
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
        public async Task<IActionResult> OnPutAsync()
        {
            var apiUrlUpdate = " ";

           
            var name = Request.Form["Discipline.Name"];
            var tasks = Request.Form["Discipline.Tasks"];
            var department = Request.Form["Discipline.Department"];
            var image = Request.Form.Files["Discipline.Image"];
            var id = Request.Form["Discipline.DisciplineId"];
            apiUrlUpdate = $"https://localhost:7149/api/Disciplines/update/{id}";

      
            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(name), "name");
                form.Add(new StringContent(tasks), "tasks");
                form.Add(new StringContent(department), "department");
                form.Add(new StringContent(id.ToString()), "id");
                if (image != null && image.Length > 0)
                {
                    var stream = new MemoryStream();
                    await image.CopyToAsync(stream);
                    stream.Position = 0;

                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);

                    form.Add(fileContent, "image", image.FileName);
                }

                var response = await client.PutAsync(apiUrlUpdate, form);

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