using DeliveryTracking.Api.Extensions;
using DeliveryTracking.Core.Contracts;
using DeliveryTracking.Core.Entities.SecurityModule;
using DeliveryTracking.Infrastructure.Data.Contexts;
using DeliveryTracking.Infrastructure.Data.DataSeed;
using DeliveryTracking.Infrastructure.ExternalService;
using DeliveryTracking.Infrastructure.Hubs;
using DeliveryTracking.Infrastructure.InternalServices;
using DeliveryTracking.Infrastructure.Repositories;
using DeliveryTracking.Services;
using DeliveryTracking.Services.Abstraction;
using DeliveryTracking.Services.Profiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace DeliveryTracking.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });

            builder.Services.AddDbContext<DeliveryTrackingDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            #region Identity and Jwt services

            builder.Services.AddIdentityCore<DeliveryTrackingUser>(
                options =>
                {
                    //MaxFailedAccessAttempts
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                }
            )
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DeliveryTrackingDbContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(options =>
              {
                  options.SaveToken = true;
                  options.TokenValidationParameters = new()
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
                      ValidAudience = builder.Configuration["JwtOptions:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"]!))
                  };
                  options.Events = new JwtBearerEvents
                  {
                      OnMessageReceived = context =>
                      {
                          var path = context.HttpContext.Request.Path;
                          if (!path.StartsWithSegments("/hubs"))
                              return Task.CompletedTask;

                          var token = context.Request.Query["access_token"];
                          if (!string.IsNullOrEmpty(token))
                              context.Token = token;
                          return Task.CompletedTask;
                      }
                  };
              }
              );

            #endregion

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IDataInitializer, DataInitializer>();

            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderNotificationService, OrderNotificationService>();
            builder.Services.AddSingleton<IConnectionMappingService, ConnectionMappingService>();

            builder.Services.AddAutoMapper(p => p.AddProfile<OrderMappingProfile>(), typeof(ServicesAssemblyReference).Assembly);

            builder.Services.AddAuthorization();
            #endregion

            var app = builder.Build();
            await app.MigrateDatabaseAsync();
            await app.SeedingIdentityData();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationHub>("/hubs/notification");
            app.MapHub<OrderTrackingHub>("/hubs/order-tracking");

            app.MapControllers();


            app.Run();
        }
    }
}
