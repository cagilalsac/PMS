using Projects.APP;
using Projects.APP.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Projects.APP.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();



// Add services to the container.
// Inversion of Control (IoC) for dependency injection:
var connectionString = builder.Configuration.GetConnectionString("ProjectsDb"); // get connection string from appsettings.json

// Inject ProjectsDb instance through service constructors with parameter of type ProjectsDb:
// SQL Server LocalDB:
//builder.Services.AddDbContext<ProjectsDb>(options => options.UseSqlServer(connectionString)); // define the DbContext with the connection string

// SQLite:
builder.Services.AddDbContext<ProjectsDb>(options => options.UseSqlite(connectionString));

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ProjectsDbService).Assembly)); // for IMediator injection in controllers

/*
 * Service Lifetimes in ASP.NET Core Dependency Injection:
 *
 * 1. AddScoped:
 *    - Lifetime: Scoped to a single HTTP request (or scope).
 *    - Behavior: Creates one instance of the service per HTTP request.
 *    - Use case: Use when you want to maintain state or dependencies that last only during a single request.
 *    - Example: DbContext, which should be shared across operations within a request.
 *
 * 2. AddSingleton:
 *    - Lifetime: Singleton for the entire application lifetime.
 *    - Behavior: Creates only one instance of the service for the whole app lifecycle.
 *    - Use case: Use for stateless services or global shared data/services.
 *    - Example: Caching services, configuration providers, logging services.
 *
 * 3. AddTransient:
 *    - Lifetime: Transient (short-lived).
 *    - Behavior: Creates a new instance every time the service is requested.
 *    - Use case: Use for lightweight, stateless services that are cheap to create.
 *    - Example: Utility/helper classes without state.
 *
 * Important Notes:
 * - Injecting a Scoped service into a Singleton can cause issues due to lifetime mismatch.
 * - ASP.NET Core DI container will warn about such mismatches.
 *
 * Summary:
 * | Method        | Lifetime                | Instance Created             | Typical Use Case                  |
 * |---------------|-------------------------|------------------------------|-----------------------------------|
 * | AddScoped     | Per HTTP request        | One instance per request     | DbContext, per-request services   |
 * | AddSingleton  | Application-wide        | One instance for app lifetime| Caching, config, logging          |
 * | AddTransient  | Every time requested    | New instance each time       | Lightweight stateless helpers     |
 */



// ======================================================
// APP SETTINGS
// ======================================================

// Access the "AppSettings" section from appsettings.json.
// This section typically holds values like JWT issuer, audience, key, expiration, etc.
var section = builder.Configuration.GetSection(nameof(AppSettings));

// Bind the configuration section directly to the static AppSettings class.
// This sets values like Issuer, Audience, SecurityKey used in token creation and validation.
section.Bind(new AppSettings());



// ======================================================
// AUTHENTICATION
// ======================================================

// Enable JWT Bearer authentication as the default scheme.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(config =>
    {
        // Define rules for validating JWT tokens.
        config.TokenValidationParameters = new TokenValidationParameters
        {
            // Match the token's issuer to the expected issuer from AppSettings.
            ValidIssuer = AppSettings.Issuer,

            // Match the token's audience to the expected audience.
            ValidAudience = AppSettings.Audience,

            // Use the symmetric key defined in AppSettings to verify the token's signature.
            IssuerSigningKey = AppSettings.SigningKey,

            // These flags ensure thorough validation of the token.
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });



// Add controllers to the service container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



// ======================================================
// SWAGGER
// ======================================================

// Configure Swagger/OpenAPI documentation, including JWT auth support in the UI.
builder.Services.AddSwaggerGen(c =>
{
    // Define the basic information for your API.
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });

    // Add the JWT Bearer scheme to the Swagger UI so tokens can be tested in requests.
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = """
        JWT Authorization header using the Bearer scheme.
        Enter your token as: Bearer your_token_here
        Example: "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        """
    });

    // Add the security requirement globally so all endpoints are secured unless specified otherwise.
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});



// Build the application.
var app = builder.Build();

// Map default endpoints for the application.
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection for the application.
app.UseHttpsRedirection();



// ======================================================
// AUTHENTICATION
// ======================================================

// Enable authentication middleware so that [Authorize] works.
app.UseAuthentication();



// Enable authorization for the application.
app.UseAuthorization();

// Map controllers to the application.
app.MapControllers();

// Run the application.
app.Run();
