
using Microsoft.EntityFrameworkCore;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<WaveMasterDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<ISharedVariableService, CommunicationService>();

            var app = builder.Build();

            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(o =>
            {
                o.AllowAnyOrigin();
                o.AllowAnyHeader();
                o.AllowAnyMethod();
            });

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            
            app.UseCors(o =>
            {
                o.AllowAnyOrigin();
                o.AllowAnyMethod();
                o.AllowAnyHeader();
            });
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}