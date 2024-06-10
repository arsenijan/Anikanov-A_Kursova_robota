
using API.Data;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", builder =>
                {
                    builder.WithOrigins("https://localhost:7228/")
                           .AllowAnyHeader()
                           .AllowAnyMethod().AllowAnyOrigin();
                });
            });
           
           

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "Artem",
                    ValidAudience = "Tamoga",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("3713ab7f791c1991d3a210c5fa68c3aa"))
                     
                };
                
            });
            
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("All", policy => policy.RequireRole("Teacher","Admin","Student","TechStaff"));
                options.AddPolicy("Personal", policy => policy.RequireRole("Teacher", "Admin","TechStaff"));
                options.AddPolicy("UniversityPersonal", policy => policy.RequireRole("Teacher", "Admin"));
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Teacher", policy => policy.RequireRole("Teacher"));
                options.AddPolicy("Student", policy => policy.RequireRole("Student"));
                options.AddPolicy("TechStaff", policy => policy.RequireRole("TechStaff"));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var ConnectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(ConnectionString,ServerVersion.AutoDetect(ConnectionString)));
            var app = builder.Build();
            app.UseCors("AllowOrigin");
           
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
             app.UseStaticFiles(); 

            app.MapControllers();

            app.Run();
        }
    }
}
