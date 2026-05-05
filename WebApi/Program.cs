using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Repository.models;
using Microsoft.OpenApi.Models;
using Service.Interfaces;
using Service.Services;
using Repository.DataRepositories;
using AutoMapper;
using CodeFirst;
using Repository.Interfaces;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("jwt:Key is not configured in appsettings.json");
            var jwtIssuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("jwt:Issuer is not configured in appsettings.json");
            var jwtAudience = jwtSection["Audience"] ?? throw new InvalidOperationException("jwt:Audience is not configured in appsettings.json");

            // CORS Configuration
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Authentication Configuration
            var key = Encoding.UTF8.GetBytes(jwtKey);

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
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Database Configuration
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

            // Services
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICandidateProfile, CandidateService>();
            builder.Services.AddScoped<IJobListings, JobListingsService>();
            builder.Services.AddScoped<IMatch, MatchService>();
            builder.Services.AddScoped<IUser, UserService>();
            builder.Services.AddScoped<ICategories, CategoryService>();
            builder.Services.AddScoped<IEmployer, EmployerService>();

            builder.Services.AddAutoMapper(typeof(Service.Services.MyMapper).Assembly);

            // Repositories
            builder.Services.AddScoped<IRepository<User>, UserRepository>();
            builder.Services.AddScoped<IRepositoryEmployer, EmployerRepository>();
            builder.Services.AddScoped<IRepository<Match>, MatchRepository>();
            builder.Services.AddScoped<IRepository<Categories>, CategoriesRepository>();
            builder.Services.AddScoped<IRepository<JobListings>, JobListingsRepository>();
            builder.Services.AddScoped<IRepository<CandidateProfiles>, CandidateProfilesRepository>();
            builder.Services.AddScoped<IRepositoryCandidateProfiles, CandidateProfilesRepository>();
            
            // Extended Repository עם Include
            builder.Services.AddScoped<JobListingsExtendedRepository>();

            var app = builder.Build();

            // Pipeline Configuration
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(); // Enable CORS
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            // Seed Data for testing
            await SeedData(app.Services);

            app.MapControllers();
            app.Run();
        }

        static async Task SeedData(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataBase>();
            
            Console.WriteLine("🌱 Starting Seed Data creation...");
            
            try
            {
                // בדיקה אם כבר יש נתונים
                var existingCandidates = await context.CandidateProfiles.AnyAsync();
                if (existingCandidates)
                {
                    Console.WriteLine("📊 Seed Data already exists, skipping...");
                    return;
                }

                // יצירת Users סינתטיים לפני יצירת CandidateProfiles - מתאים למבנה האמיתי של User
                var users = new List<User>
                {
                    new User { Id = 1001, Name = "דני כהן", Email = "dani.cohen@test.com", PasswordHash = "hashed_password_1001", UserType = UserRole.Candidate, IsEnable = true },
                    new User { Id = 1002, Name = "שרה לוי", Email = "sara.levi@test.com", PasswordHash = "hashed_password_1002", UserType = UserRole.Candidate, IsEnable = true },
                    new User { Id = 1003, Name = "משה ישראלי", Email = "moshe.israeli@test.com", PasswordHash = "hashed_password_1003", UserType = UserRole.Candidate, IsEnable = true }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Created {users.Count} users");

                // יצירת Candidates לבדיקה - מתאים למבנה האמיתי של CandidateProfiles
                var candidates = new List<CandidateProfiles>
                {
                    new CandidateProfiles
                    {
                        UserId = 1001, // יצירת UserId סינתטי
                        CategoryId = 3,
                        City = "תל אביב",
                        MaxDistance = 50,
                        MinHourlyRate = 150,
                        activity = true,
                        level = elevel.Medium,
                        IsRemoteOnly = false,
                        Withpepole = true
                    },
                    new CandidateProfiles
                    {
                        UserId = 1002, // יצירת UserId סינתטי
                        CategoryId = 2,
                        City = "ירושלים",
                        MaxDistance = 30,
                        MinHourlyRate = 120,
                        activity = true,
                        level = elevel.Easy,
                        IsRemoteOnly = true,
                        Withpepole = false
                    },
                    new CandidateProfiles
                    {
                        UserId = 1003, // יצירת UserId סינתטי
                        CategoryId = 1,
                        City = "חיפה",
                        MaxDistance = 100,
                        MinHourlyRate = 180,
                        activity = true,
                        level = elevel.Hard,
                        IsRemoteOnly = false,
                        Withpepole = true
                    }
                };

                await context.CandidateProfiles.AddRangeAsync(candidates);
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Created {candidates.Count} candidates");

                // יצירת Matches עם JobId 2102
                var matches = new List<Match>();
                var jobId = 2102; // ה-JobId הקיים במערכת
                
                foreach (var candidate in candidates)
                {
                    matches.Add(new Match
                    {
                        CandidateId = candidate.Id,
                        JobId = jobId,
                        MatchScore = new Random().NextDouble() * 40 + 60, // ציון בין 60-100
                        MatchDate = DateTime.Now.AddHours(-new Random().Next(1, 24)),
                        IsSelectedByAlgorithm = true,
                        Status = "pending" // סטטוס התחלתי
                    });
                }

                await context.Match.AddRangeAsync(matches);
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Created {matches.Count} matches for JobId {jobId}");

                Console.WriteLine("🎉 Seed Data completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating Seed Data: {ex.Message}");
            }
        }
    }
}
