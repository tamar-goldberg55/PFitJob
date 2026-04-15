using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // חובה להוסיף
using Microsoft.AspNetCore.Authentication.JwtBearer; // חובה להוסיף
using System.Text;
using Repository.models;
using Service.Interfaces;
using Service.Services;
using Repository.Interfaces;
using Repository.DataRepositories;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. הגדרת CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // 2. הגדרת Authentication (אימות) - זה החלק שחסר לך!
            // וודאי שהמפתח כאן זהה בדיוק למפתח ב-TokenService
            var key = Encoding.UTF8.GetBytes("YourSuperSecretKeyMustBeAtLeast32CharactersLong");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "YourIssuer", // זהה ל-TokenService
                    ValidateAudience = true,
                    ValidAudience = "YourAudience", // זהה ל-TokenService
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // 3. רישום שירותים (DI)
            builder.Services.AddDbContext<CodeFirst.DataBase>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<Repository.Interfaces.IContext, CodeFirst.DataBase>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // רישום Services
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICandidateProfile, CandidateService>();
            builder.Services.AddScoped<IJobListings, JobListingsService>();
            builder.Services.AddScoped<IMatch, MatchService>();
            builder.Services.AddScoped<IUser, UserService>();
            builder.Services.AddScoped<ICategories, CategoryService>();
            builder.Services.AddScoped<IEmployer, EmployerService>();

            builder.Services.AddAutoMapper(typeof(Service.Services.MyMapper).Assembly);

            // רישום Repositories
            builder.Services.AddScoped<IRepository<User>, UserRepository>();
            builder.Services.AddScoped<IRepository<Employer>, EmployerRepository>();
            builder.Services.AddScoped<IRepository<Match>, MatchRepository>();
            builder.Services.AddScoped<IRepository<Categories>, CategoriesRepository>();
            builder.Services.AddScoped<IRepository<JobListings>, JobListingsRepository>();
            builder.Services.AddScoped<IRepository<CandidateProfiles>, CandidateProfilesRepository>();

            var app = builder.Build();

            // 4. הגדרת ה-Pipeline (הסדר כאן קריטי!)
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(); // קודם כל CORS
            app.UseHttpsRedirection();

            app.UseAuthentication(); // 1. בדיקה מי המשתמש (חובה לפני Authorization)
            app.UseAuthorization();  // 2. בדיקה מה מותר לו לעשות

            app.MapControllers();
            app.Run();
        }
    }
}