using Inmo24.Application;
using Inmo24.Application.Services.Implementations;
using Inmo24.Application.Services.Interfaces;
using Inmo24.Domain.Models;
using System.Text.Json.Serialization;

namespace Inmo24.API.Config;

public static class ServicesConfig
{
    public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        // services.AddSignalR();
        services.AddMemoryCache();

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                if (allowedOrigins != null && allowedOrigins.Any())
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                }
                else
                {
                    builder.SetIsOriginAllowed(_ => true)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                }
            });
        });

        // QuestPDF.Settings.License = LicenseType.Community;

        services.AddSwagger();

        services.AddJwt(configuration);

        services.AddInternalServices();
        services.BindAppSettings(configuration);
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        // services.AddHealthChecks(configuration);


        services.AddDbContext<InmobiliariaContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        // services.AddConfiguration(configuration);
    }

    // private static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddHealthChecks()
    //         .AddCheck<DatabaseFacturacionHealthCkeck>(nameof(DatabaseFacturacionHealthCkeck));
    // }

    private static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"] ?? "Inmo24",
                ValidAudience = configuration["Jwt:Audience"] ?? "Inmo24",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                    configuration["Jwt:SecretKey"] ?? "YourSecretKeyHere12345")),
                ClockSkew = TimeSpan.Zero,
            };

            options.Events = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    // Configurar si usas SignalR:
                    // if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub/dashboard"))
                    // {
                    //     context.Token = accessToken;
                    // }
                    return Task.CompletedTask;
                },

                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    return context.Response.WriteAsync(JsonSerializer.Serialize(
                        OperationResponse<object>.CreateBuilder().WithCode(401)
                            .WithMessage("No estás autenticado.").Build()));
                },

                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(JsonSerializer.Serialize(
                        OperationResponse<object>.CreateBuilder().WithCode(403)
                            .WithMessage("No tienes permisos para realizar esta acción.").Build()));
                },
            };
        });
    }

    // private static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    // {
    //     // Sequence.Initialize(configuration);
    //     // var context = services.BuildServiceProvider().GetRequiredService<FacturacionContext>();
    // }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "Inmo24 API", 
                Version = "v1" 
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Ingrese el token de autenticación en el siguiente formato: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            // c.OperationFilter<FileUploadOperationFilter>();

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
           // c.IncludeXmlComments(xmlPath);
        });
    }

    private static void AddInternalServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPropiedadService, PropiedadesService>();
        services.AddScoped<IStorageService, CloudflareR2StorageService>();
        services.AddScoped<ICatalogoService, CatalogoService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IOperacionComercialService, OperacionComercialService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IDocumentosService, DocumentosService>();
        services.AddScoped<IVisitaService, VisitaService>();
        services.AddScoped<IMensajeService, MensajeService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IInmobiliariaService, InmobiliariaService>();
        services.AddScoped<IZonaService, ZonaService>();
        services.AddScoped<IBotConfigService, BotConfigService>();
    }

    private static void BindAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var dataBase = new AppSettings.DataBase();
        var jwt = new AppSettings.Jwt();

        configuration.Bind("ConnectionStrings", dataBase);
        configuration.Bind("Jwt", jwt);
    }
}
