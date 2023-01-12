using Microsoft.AspNetCore.ResponseCompression;
using Xray.Hope.Service.Ssh;
using Xray.Hope.Service.Telnet;
using Xray.Hope.Service.Xui;
using Xray.Hope.Web.Server.Hubs;

namespace Xray.Hope.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddSignalR();
            builder.Services.AddSingleton<HopeSshClient>();
            builder.Services.AddSingleton<XuiInstallScriptGenerator>();
            builder.Services.AddSingleton<TelnetClient>();
            builder.Services.AddScoped<HopeXuiHttpClient>();
            builder.Services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions();

            // Disables adaptive sampling.
            aiOptions.EnableAdaptiveSampling = false;

            builder.Services.AddApplicationInsightsTelemetry();

            var app = builder.Build();
            //app.UseResponseCompression();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();


            app.MapRazorPages();
            app.MapControllers();
            app.MapHub<ConsoleHub>("/hopehub");
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}