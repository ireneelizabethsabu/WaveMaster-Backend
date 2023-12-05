using Microsoft.EntityFrameworkCore;
using Serilog;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend
{
    /// <summary>
    /// Main program class for the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method to start the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            // Create a builder for the web application.
            var builder = WebApplication.CreateBuilder(args);

            // Register controllers in the application.
            builder.Services.AddControllers();

            // Add authorization services to the container.
            builder.Services.AddAuthorization();

            // Add Entity Framework DbContext to the container with a singleton lifetime.
            builder.Services.AddDbContext<WaveMasterDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")), ServiceLifetime.Singleton);

            // Add API Explorer for endpoints.
            builder.Services.AddEndpointsApiExplorer();

            // Add Swagger documentation generation.
            builder.Services.AddSwaggerGen();

            // Add singleton services for various functionalities.
            builder.Services.AddSingleton<ISerialPortService, SerialPortService>();
            builder.Services.AddSingleton<IReadService, ReadService>();
            builder.Services.AddSingleton<IObserverService, ObserverService>();
            builder.Services.AddSingleton<IFileService, FileService>();
            builder.Services.AddSignalR();

            // Build the application.
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger and SwaggerUI for development environment.
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Enable CORS for all origins, headers, and methods.
            app.UseCors(o =>
            {
                o.AllowAnyOrigin();
                o.AllowAnyHeader();
                o.AllowAnyMethod();
            });

            // Configure Serilog for logging.
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.File("logs/demo.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();


            // Enable authorization and map controllers and SignalR hub.
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<PlotDataHub>("/plotValue");

            // Run the application.
            app.Run();
        }

    }
}