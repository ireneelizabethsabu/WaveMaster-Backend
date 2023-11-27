
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //CreateHostBuilder(args).Build().Run();
            builder.Services.AddControllers();

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<WaveMasterDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")), ServiceLifetime.Singleton);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<ISharedVariableService, SerialPortService>();
            builder.Services.AddSingleton<IReadService, ReadService>();
            builder.Services.AddSingleton<IObserverService, ObserverService>();

            builder.Services.AddSignalR();

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

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.File("logs/demo.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();



            //Log.Information("Hello, world!");

            //int a = 10, b = 0;
            //try
            //{
            //    Log.Debug("Dividing {A} by {B}", a, b);
            //    Console.WriteLine(a / b);
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(ex, "Something went wrong");
            //}
            //finally
            //{
            //    Log.CloseAndFlush();
            //}

            

            //app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<PlotDataHub>("/plotValue");
            app.Run();
        }
        
    }
}