using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using System.Net;
using System.Threading.RateLimiting;
using Xray.Hope.Service.Ssh;
using Xray.Hope.Service.Telnet;
using Xray.Hope.Service.Xui;
using Xray.Hope.Web.Server.Hubs;
using Xray.Hope.Web.Server.Models.Configurations;

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

            var aiOptions = new ApplicationInsightsServiceOptions
            {
                // Disables adaptive sampling.
                EnableAdaptiveSampling = false
            };

            builder.Services.AddApplicationInsightsTelemetry(aiOptions);

            var rateLimitOptions = new HopeRateLimiterOptions();
            builder.Configuration.GetSection("SshExecutionRateLimiterOptions").Bind(rateLimitOptions);

            var userPolicyName = "IP_BASED_RATE_LIMITER";
            builder.Services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.AddPolicy(userPolicyName, context =>
                {
                    var remoteIpAddress = context.Connection.RemoteIpAddress;

                    if (!IPAddress.IsLoopback(remoteIpAddress!))
                    {
                        return RateLimitPartition.GetSlidingWindowLimiter(remoteIpAddress,
                            _ => new SlidingWindowRateLimiterOptions
                            {
                                PermitLimit = rateLimitOptions.PermitLimit,
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = rateLimitOptions.QueueLimit,
                                Window = TimeSpan.FromSeconds(rateLimitOptions.Window),
                                SegmentsPerWindow = rateLimitOptions.SegmentsPerWindow
                            });
                    }

                    return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseResponseCompression();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseRateLimiter();

            app.MapRazorPages();
            app.MapControllers();

            app.MapHub<ConsoleHub>("/hopehub");
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}