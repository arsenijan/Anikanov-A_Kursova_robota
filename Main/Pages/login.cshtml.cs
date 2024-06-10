using API.Models;
using Main.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Main.Pages
{
    public class loginModel : PageModel
    {
        [BindProperty]
        public User User { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var apiUrlPost = "https://localhost:7149/api/Users/create";

            using (var client = new HttpClient())
            {


                var form = new MultipartFormDataContent();

                form.Add(new StringContent(User.FullName), "fullname");
                form.Add(new StringContent(User.Password.ToString()), "password");
                form.Add(new StringContent(User.Role.ToString()), "role");

              

                var response = await client.PostAsync(apiUrlPost, form);
                Console.WriteLine(response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    return Redirect("/index");
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
