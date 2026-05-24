using Debugged.Application;
using Debugged.Infrastructure;
using Debugged.API.Middleware;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Application + Infrastructure layers (CQRS, EF Core, repositories, Identity, JWT).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

// Swagger generates the OpenAPI spec and serves the interactive UI at /swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Debugged API",
        Version = "v1",
        Description = "Bug Knowledge Base API — archive resolved bugs with their solutions and find similar issues by tags and error messages."
    });

    // Tells Swagger that the API uses JWT Bearer auth — adds an "Authorize" button to the UI.
    // Users paste their token once and it's sent on every protected request from the UI.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "JWT Authorization header. Format: 'Bearer {token}'"
    });

    // Apply the Bearer requirement globally — every endpoint accepts the token.
    // Endpoints without [Authorize] still work without one.
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

var app = builder.Build();

// Run seeder on startup — creates roles and a demo admin user if missing.
// Wrapped in a scope because RoleManager/UserManager are scoped services.
using (var scope = app.Services.CreateScope())
{
    await Debugged.Infrastructure.Persistence.DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

// First in the pipeline — wraps everything below so all exceptions are caught.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Debugged API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Authentication MUST come before Authorization — otherwise [Authorize] runs on an unauthenticated
// request and rejects it before the JWT is parsed.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();