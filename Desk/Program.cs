using Desk.Data;
using Desk.Models;
using Desk.Middleware;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Desk.Services;
using Desk.Contracts.Services;
using Desk.Wrappers;


namespace Desk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate();

            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy.
                options.FallbackPolicy = options.DefaultPolicy;
            });


            // Configure the database

            // SQL Server
            builder.Services.AddDbContext<DeskContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null)
                    ));

            // Sqlite
            //builder.Services.AddDbContext<DeskContext>(options =>
            //    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'TestConnection' not found.")));

            // Use Serilog for logging
            builder.Host.UseSerilog((context, services, configuration) => configuration
                   .ReadFrom.Configuration(context.Configuration));

            builder.Services.AddRazorPages();

            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);     // the default is 30 days
            });

            // For accessing HttpsContext - User and connection information
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserService, UserService>();

            // For using the SMTP
            builder.Services.AddScoped<ISmtpClient, SmtpClientWrapper>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            // For accessing HTTP commands to get JSON data
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                //app.UseExceptionHandler("/Error");
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");

                // HTTP Strict Transport Security
                // Helps protect your website against certain types of attacks, such as protocol downgrade attacks and cookie hijacking.
                app.UseHsts();

                app.UseMiddleware<ExceptionHandlingMiddleware>();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseMiddleware<ClientUserNameLoggingMiddleware>();
            app.UseMiddleware<ClientIpLoggingMiddleware>();

            // Enable Serilog request logging
            app.UseSerilogRequestLogging();

            try
            {
                Log.Information("Starting up the application");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
