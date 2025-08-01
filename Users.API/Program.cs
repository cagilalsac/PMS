// Importing required namespaces for working with Users, authentication, EF Core, JWT, and Swagger
using Users.APP;
using Users.APP.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Users.APP.Services;

var builder = WebApplication.CreateBuilder(args);

// Adds service defaults like configuration, health checks, telemetry, etc.
// This is a helper method often defined in .NET template apps or extensions.
builder.AddServiceDefaults();



// ======================================================
// IoC (Inversion of Control) DATABASE CONFIGURATION
// ======================================================

// Fetch the connection string named "UsersDb" from appsettings.json or environment variables.
// This determines where the application stores and retrieves user-related data.
var connectionString = builder.Configuration.GetConnectionString("UsersDb");

// Register the UsersDb context for DbContext injection in services for Entity Framework Core using SQL Server.
// This enables the app to interact with the Users database via LINQ and EF models.
//builder.Services.AddDbContext<DbContext, UsersDb>(options => options.UseSqlServer(connectionString));

// Alternative: Uncomment to use SQLite instead of SQL Server
builder.Services.AddDbContext<DbContext, UsersDb>(options => options.UseSqlite(connectionString));



// ======================================================
// IoC (Inversion of Control) MEDIATR CONFIGURATION
// ======================================================

// Register MediatR services from the assembly containing UsersDbHandler.
// This enables decoupled request/response logic using IRequest<T> and IRequestHandler<TRequest, TResponse>.
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UserService).Assembly));

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



// ======================================================
// CONTROLLERS CONFIGURATION
// ======================================================

// Register controller services so that they can handle incoming HTTP requests.
builder.Services.AddControllers();

// Enables API explorer so endpoints can be discovered.
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



// ======================================================
// APPLICATION BUILD AND MIDDLEWARE PIPELINE
// ======================================================

// Build the configured application.
var app = builder.Build();

// Map default endpoints (if defined in service defaults or extensions)
app.MapDefaultEndpoints();

// Use Swagger and Swagger UI only in development for API documentation and testing.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP requests to HTTPS for security.
app.UseHttpsRedirection();



// ======================================================
// AUTHENTICATION
// ======================================================

// Enable authentication middleware so that [Authorize] works.
app.UseAuthentication();



// Enable authorization middleware to enforce roles and permissions.
app.UseAuthorization();

// Map attribute-based controllers to routes (e.g., [Route], [HttpGet], etc.)
app.MapControllers();

// Start the application and begin processing requests.
app.Run();