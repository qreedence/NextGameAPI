using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.Data.Repositories;
using NextGameAPI.Hubs;
using NextGameAPI.Services.Circles;
using NextGameAPI.Services.DTOConverters;
using NextGameAPI.Services.Email;
using NextGameAPI.Services.IGDB;
using NextGameAPI.Services.Notifications;
using NextGameAPI.Services.Transactions;
using NextGameAPI.Services.UploadThing;
using Scalar.AspNetCore;

namespace NextGameAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            //DotNetEnv
            Env.Load();

            //DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(Environment.GetEnvironmentVariable("connection-string")));


            //Model Services
            builder.Services.AddTransient<IExternalLoginToken, ExternalLoginTokenRepository>();
            builder.Services.AddTransient<IUserSettings, UserSettingsRepository>();
            builder.Services.AddTransient<IPasswordResetToken, PasswordResetTokenRepository>();
            builder.Services.AddTransient<IFriendship, FriendshipRepository>();
            builder.Services.AddTransient<IFriendRequest, FriendRequestRepository>();
            builder.Services.AddTransient<IUser, UserRepository>();
            builder.Services.AddTransient<INotification, NotificationRepository>();
            builder.Services.AddTransient<ICircle, CircleRepository>();
            builder.Services.AddTransient<ICircleMember, CircleMemberRepository>();
            builder.Services.AddTransient<ICircleInvitation, CircleInvitationRepository>();
            builder.Services.AddTransient<ITwitchAccessToken, TwitchAccessTokenRepository>();

            //HTTP Clients
            builder.Services.AddHttpClient("IGDBAuthClient", client =>
            {
                client.BaseAddress = new Uri("https://id.twitch.tv/oauth2/");
            });

            builder.Services.AddHttpClient("IGDBClient", client =>
            {
                client.BaseAddress = new Uri("https://api.igdb.com/v4/");
                client.DefaultRequestHeaders.Add("Client-ID", Environment.GetEnvironmentVariable("igdb-client-id"));
            });

            //SignalR
            builder.Services.AddSignalR();

            //Identity
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                //password
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                //lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                //email
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            });

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.LoginPath = "/api/auth/login";
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    };
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Environment.GetEnvironmentVariable("google-client-id")!;
                    googleOptions.ClientSecret = Environment.GetEnvironmentVariable("google-client-secret")!;
                    googleOptions.SignInScheme = Microsoft.AspNetCore.Identity.IdentityConstants.ExternalScheme;
                });

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            });

            builder.Services.AddAuthorization();

            //CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(Environment.GetEnvironmentVariable("cors-client-https-url")!, Environment.GetEnvironmentVariable("cors-client-http-url")!)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            //Transaction Service
            builder.Services.AddTransient<TransactionService>();

            //UploadThing
            builder.Services.AddTransient<UploadThingHelpers>();
            builder.Services.AddTransient<UploadThingService>();

            //Email Service
            builder.Services.AddTransient<EmailService>();

            //Notification Services
            builder.Services.AddTransient<ISignalRNotificationDispatcher, SignalRNotificationDispatcher>();
            builder.Services.AddTransient<NotificationService>();

            //Circle Service
            builder.Services.AddTransient<CircleService>();

            //IGDB
            builder.Services.AddTransient<TwitchAccessTokenService>();
            builder.Services.AddTransient<GameService>();

            //DTO Converter Services
            builder.Services.AddTransient<UserConverter>();
            builder.Services.AddTransient<CircleConverter>();

            var app = builder.Build();

            app.UseCors();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<NotificationHub>("/notificationHub");

            app.Run();
        }
    }
}
