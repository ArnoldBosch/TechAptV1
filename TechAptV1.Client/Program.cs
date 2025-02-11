// Copyright © 2025 Always Active Technologies PTY Ltd

using Serilog;
using TechAptV1.Client.Components;
using TechAptV1.Client.Services;
using TechAptV1.Client.Services.Interfaces;
using TechAptV1.Client.Services.Serializers;

namespace TechAptV1.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.Title = "Tech Apt V1";

                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddSerilog(lc => lc
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .ReadFrom.Configuration(builder.Configuration));

                // Add services to the container.
                builder.Services.AddRazorComponents().AddInteractiveServerComponents();
                builder.Services.AddScoped<IDataService, DataService>();
                builder.Services.AddScoped<IBatchDataService, DataServiceSQL>();
                builder.Services.AddScoped<IThreadingService, ThreadingService>();
                builder.Services.AddScoped<IDataSerializer, XmlDataSerializer>();
                builder.Services.AddScoped<IDataSerializer, BinaryDataSerializer>();
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                }

                app.UseStaticFiles();
                app.UseAntiforgery();

                app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

                app.Run();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Fatal exception in Program");
                Console.WriteLine(exception);
            }
        }
    }
}
