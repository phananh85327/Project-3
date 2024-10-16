
using EC.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace EC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Configuration.AddJsonFile("gitignore.json", optional: false, reloadOnChange: true);

            // Add DB (Method 1)
            //builder.Services.AddDbContext<DataContext>();
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
            );

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"] ?? string.Empty))
                };
            });
            builder.Services.AddSingleton<JWTService>();

            builder.Services.AddHostedService<NotificationService>();

            builder.Services.AddLogging();

            // Log file path
            var baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\.."));
            var logFilePathConfig = builder.Configuration["LogFilePath"] ?? string.Empty;

            // Log name
            var logFileName = string.Format("log_{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
            var logFilePath = Path.Combine(baseDir, logFilePathConfig, logFileName);

            // Configure Serilog nuget
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

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
