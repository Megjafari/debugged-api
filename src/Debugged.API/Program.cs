using Debugged.Application;
using Debugged.Infrastructure;
using Debugged.API.Middleware;
var builder = WebApplication.CreateBuilder(args);

// Application + Infrastructure layers (CQRS, EF Core, repositories, etc.)
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
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger only enabled in development — production exposure can leak schema details.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Debugged API v1");
        options.RoutePrefix = "swagger"; // UI available at /swagger
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();