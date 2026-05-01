using System.Text;
using DevEval.Application;
using DevEval.Application.Products.Validators;
using DevEval.IoC;
using DevEval.ORM.Contexts;
using DevEval.WebApi.Middleware;
using DevEval.WebApi.Utils;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rebus.Config;
using Rebus.Transport.InMem;

namespace DevEval.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            var app = builder.Build();

            await ConfigureMiddleware(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            ConfigureSwagger(builder);
            ConfigureDatabase(builder);
            ConfigureDependencyInjection(builder);
            ConfigureFluentValidation(builder);
            ConfigureRouting(builder);
            ConfigureMediatR(builder);
            ConfigureAuthentication(builder);
            ConfigureRebus(builder);
        }

        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Dev Eval", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));
        }

        private static void ConfigureDependencyInjection(WebApplicationBuilder builder)
        {
            builder.Services.RegisterServices();
        }

        private static void ConfigureFluentValidation(WebApplicationBuilder builder)
        {
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
        }

        private static void ConfigureRouting(WebApplicationBuilder builder)
        {
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        private static void ConfigureMediatR(WebApplicationBuilder builder)
        {
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];
            var jwtKey = builder.Configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT configuration is missing required values (Issuer, Audience, Key).");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });
        }


        private static void ConfigureRebus(WebApplicationBuilder builder)
        {
            var inMemNetwork = new InMemNetwork();

            builder.Services.AddRebus(configure => configure
                .Transport(t => t.UseInMemoryTransportAsOneWayClient(inMemNetwork))
                .Options(o => o.LogPipeline(verbose: false))
            );
        }

        private static async Task ApplyMigrations(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();

            try
            {
                dbContext.Database.Migrate();
                Console.WriteLine("Database migration applied successfully.");

                await AdminUserInitializer.EnsureAdminUserExists(app.Services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while applying database migrations: {ex.Message}");
                throw;
            }
        }


        private static async Task ConfigureMiddleware(WebApplication app)
        {
            await ApplyMigrations(app);

            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "DevEval API v1");
                    options.RoutePrefix = string.Empty;
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
