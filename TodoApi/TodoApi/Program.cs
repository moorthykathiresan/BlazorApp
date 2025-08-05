using NSwag.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(); // Adds NSwag OpenAPI document generation
// NSwag does not require AddEndpointsApiExplorer or AddSwaggerGen

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(); // Default: /swagger/v1/swagger.json
    app.UseSwaggerUi(); // Default: /swagger
    app.UseOpenApi(settings => settings.Path = "/swagger/v1/swagger.yaml"); // Serves YAML at /swagger/v1/swagger.yaml
}

// ...existing code...
app.MapControllers();
app.Run();
