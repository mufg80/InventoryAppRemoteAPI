
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace InventoryAppRemoteAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add cert for android develepment

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(7113, listenOptions => // HTTPS
                {
                    listenOptions.UseHttps("localhost.pfx", "kyleink1"); 
                });
            });

            // Add services to the container.
            // 
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                // Define API key security scheme
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "Encrypted API key needed to access the endpoints. X-Encrypted-Api-Key: <encrypted_key>",
                    In = ParameterLocation.Header,
                    Name = "X-encrypted-api-key",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKey"
                });

                // Apply the security scheme globally
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new List<string>()
        }
    });



            });

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");

                    c.InjectStylesheet("/swagger-ui/custom.css");
                });
            }

            app.Use(async (context, next) =>
            {
                // Check for the custom header
                if (!context.Request.Headers.TryGetValue("X-Encrypted-Api-Key", out var apiKey) || !Util.DecryptKey.IsValidKey(apiKey))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Invalid API Key");
                    return;
                }
                
               
                // Call the next middleware in the pipeline
                await next.Invoke();
            });


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
