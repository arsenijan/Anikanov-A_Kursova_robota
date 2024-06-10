using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Dictionary<string, string> loginData)
    {
        if (loginData == null || !loginData.ContainsKey("username") || !loginData.ContainsKey("password"))
        {
            return BadRequest("Invalid login request");
        }

        var username = loginData["username"];
        var password = loginData["password"];

        var user = _context.Users.SingleOrDefault(x => x.FullName == username && x.Password == password);

        if (user == null)
        {
            return Unauthorized("Invalid username or password");
        }

        var key = Encoding.UTF8.GetBytes("3713ab7f791c1991d3a210c5fa68c3aa");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
               {
                 new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(90),
            Issuer = "Artem",
            Audience = "Tamoga",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
      


        return Ok(tokenHandler.WriteToken(token));
         
                
    }
}
