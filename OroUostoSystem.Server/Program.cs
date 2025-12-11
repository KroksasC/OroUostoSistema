
using Microsoft.OpenApi.Models;
using OroUostoSystem.Server.Extensions;

namespace OroUostoSystem.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.WebHost.UseUrls("http://0.0.0.0:80");
            // Add services to the container.
            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });



            var app = builder.Build();

            // -----------------------------
            // Run DB initializer correctly
            // -----------------------------
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var dbInitializer = services.GetRequiredService<OroUostoSystem.Server.DbInitializer.IDbinitializer>();
                    dbInitializer.Initialize().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred during database initialization.");
                }
            }

            // -----------------------------
            // Middleware
            // -----------------------------
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors(cors =>
                cors.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("https://localhost:52332"));


            app.Use(async (context, next) =>
            {
                Console.WriteLine($"=== REQUEST DEBUG ===");
                Console.WriteLine($"Path: {context.Request.Path}");
                Console.WriteLine($"Method: {context.Request.Method}");

                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"Auth Header Present: {!string.IsNullOrEmpty(authHeader)}");
                if (!string.IsNullOrEmpty(authHeader))
                {
                    Console.WriteLine($"Auth Header: {authHeader.Substring(0, Math.Min(50, authHeader.Length))}...");
                }

                await next();

                Console.WriteLine($"Response Status: {context.Response.StatusCode}");
                Console.WriteLine($"=== END REQUEST DEBUG ===");
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
