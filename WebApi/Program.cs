
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.DataRepositories;
using Repository.Interfaces;
using Repository.models;
using Service.Interfaces;
using Service.Services;


namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // הוספת ה-DbContext למערכת
            builder.Services.AddDbContext<CodeFirst.DataBase>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<Repository.Interfaces.IContext, CodeFirst.DataBase>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICandidateProfile, CandidateService>(); // <-- זו השורה החדשה
            // רישום השירותים החדשים שהוספת
            builder.Services.AddScoped<IJobListings, JobListingsService>(); // ודא שזה שם המחלקה המממשת
            builder.Services.AddScoped<IMatch, MatchService>();     // ודא שזה שם המחלקה המממשת
            builder.Services.AddScoped<IUser, UserService>();
            builder.Services.AddScoped<ICategories, CategoryService>();

            // בנוסף, ודא ש-AutoMapper רשום (מכיוון שהוספת IMapper)
            // builder.Services.AddAutoMapper(typeof(Program));
            //builder.Services.AddAutoMapper(typeof(MyMapper));
            builder.Services.AddAutoMapper(typeof(Service.Services.MyMapper).Assembly);

            // רישום ה-Repositories עבור כל מודל בנפרד
            builder.Services.AddScoped<IRepository<User>, UserRepository>();
            builder.Services.AddScoped<IRepository<Employer>, EmployerRepository>();
            builder.Services.AddScoped<IRepository<Match>, MatchRepository>();
            builder.Services.AddScoped<IRepository<Categories>, CategoriesRepository>();
            builder.Services.AddScoped<IRepository<JobListings>, JobListingsRepository>();
            builder.Services.AddScoped<IRepository<CandidateProfiles>, CandidateProfilesRepository>();
            //builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository.DataRepositories<>));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
           
        }
    }
}
