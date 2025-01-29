using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data;
using NextGameAPI.Data.Models;
using Scalar.AspNetCore;
using System;

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

            //Identity
            builder.Services.AddIdentityCore<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication();

            builder.Services.AddAuthorization();

            //CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(Environment.GetEnvironmentVariable("cors-client-https-url")!, Environment.GetEnvironmentVariable("cors-client-http-url")!)
                    .AllowAnyOrigin()
                    .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            app.UseCors();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}
