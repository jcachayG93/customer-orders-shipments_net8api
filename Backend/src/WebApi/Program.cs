using Microsoft.EntityFrameworkCore;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Adds this application specific services.
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetService<AppDbContext>()!;
    if (!dbContext.Database.CanConnect())
    {
        throw new InvalidOperationException("Can't connect to the database. This application requires a Postgres database." +
                                            "There is a docker-compose.yml file so you can create a database in docker. You can" +
                                            "also modify the appsettings.json file to specify a different database.");
    }
    /*
     * Deletes the database each time the application starts, ths is something I do for development, while running in docker,
     * eventually will remove this code.
     */
    dbContext!.Database.EnsureDeleted();
    dbContext!.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
namespace WebApi
{
    public partial class Program
    {
    
    }
}