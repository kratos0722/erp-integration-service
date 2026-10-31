using ErpIntegration.Api.Middleware;
using ErpIntegration.Application;
using ErpIntegration.Infrastructure;
using ErpIntegration.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Create the schema on startup. The sample uses SQLite so the demo is zero-config;
// a production deployment would run EF Core migrations instead.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
    db.Database.EnsureCreated();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

// Exposed so the integration test project can host the app via WebApplicationFactory.
public partial class Program { }
