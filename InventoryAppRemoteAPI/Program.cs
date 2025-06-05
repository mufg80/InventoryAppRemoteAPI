using InventoryAppRemoteAPI.DBAccess;
using InventoryAppRemoteAPI.Util;
using Microsoft.OpenApi.Models;

namespace InventoryAppRemoteAPI
{
    /// <summary>
    /// The entry point for the InventoryAppRemoteAPI application.
    /// Responsible for configuring services, middleware, and security.
    /// Author: Shannon Musgrave
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method that initializes and runs the web application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register DecryptKey and DBAccesser services for dependency injection
            builder.Services.AddSingleton<DecryptKey>();
            builder.Services.AddSingleton<DBAccesser>();

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configure Swagger/OpenAPI documentation
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Define API key security scheme for authentication This ensures Swagger will authorize requests using the API key
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "Encrypted API key needed to access the endpoints. X-Encrypted-Api-Key: <encrypted_key>",
                    In = ParameterLocation.Header,
                    Name = "X-Encrypted-Api-Key",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKey"
                });

                // Apply the security scheme globally
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    new List<string>()
                }});
            });

            // Configure HTTPS and certificate settings for Android development only.
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(7113, listenOptions =>
                {
                    listenOptions.UseHttps("localhost.pfx", "kyleink1");
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
                    c.InjectStylesheet("/swagger-ui/custom.css"); // Custom styling for Swagger UI
                });
            }

            // Middleware to validate API key before processing requests
            app.Use(async (context, next) =>
            {
                var decryptKeyService = context.RequestServices.GetRequiredService<DecryptKey>();

                // Check if the request contains a valid API key header this checks that the api key has been sent by android device
                // andn returns 401 Unauthorized if not present or invalid.
                if (!context.Request.Headers.TryGetValue("X-Encrypted-Api-Key", out var apiKey) || !decryptKeyService.IsValidKey(apiKey))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Invalid API Key");
                    return;
                }

                // Proceed to next middleware in the pipeline
                await next.Invoke();
            });

            app.UseHttpsRedirection(); // Redirect HTTP traffic to HTTPS
            app.UseAuthorization(); // Apply authorization policies
            app.MapControllers(); // Map API controllers

            // Start the application
            app.Run();
        }
    }
}
