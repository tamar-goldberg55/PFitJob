using Microsoft.AspNetCore.Authentication.JwtBearer; // חובה להוסיף
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // חובה להוסיף
using Microsoft.OpenApi.Models;
using Repository.DataRepositories;
using Repository.Interfaces;
using Repository.models;
using Service.Interfaces;
using Service.Services;
using System.Text;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("jwt:Key is not configured in appsettings.json");

            var jwtIssuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("jwt:Issuer is not configured in appsettings.json");
            var jwtAudience = jwtSection["Audience"] ?? throw new InvalidOperationException("jwt:Audience is not configured in appsettings.json");
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
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,// זהה ל-TokenService
                    ValidateAudience = true,
                    ValidAudience = jwtAudience, // זהה ל-TokenService
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
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1 API", Version = "v1" });

                var bearerScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter 'Bearer' [space] and then your valid JWT token.\r\n\r\nExample: \"Bearer eyJhb...\"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                options.AddSecurityDefinition("Bearer", bearerScheme);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
            });

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
            builder.Services.AddScoped<IRepositoryEmployer, EmployerRepository>();
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