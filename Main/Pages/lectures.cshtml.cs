using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Main.Pages
{
    public class lecturesModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public lecturesModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public List<User> Teachers { get; private set; }
        public async Task<IActionResult> OnGetAsync() 
        {
            var response = await _httpClient.GetAsync("https://localhost:7149/api/Users/get");
            if (response.IsSuccessStatusCode)
            {

                
                var jsonData = await response.Content.ReadAsStringAsync();
               var teachers = JsonConvert.DeserializeObject<List<User>>(jsonData);
                Teachers = teachers.Where(x => x.Role == "Викладач").ToList();
                foreach(var item in Teachers)
                {
                    Console.WriteLine(item.FullName);
                }
             
            }
            else
            {
               
                Teachers = new List<User>();
            }
            return Page();
        }
    }
}
