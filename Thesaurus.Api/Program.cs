using Thesaurus.Core.Data;
using Microsoft.EntityFrameworkCore;
using Thesaurus.core.Interfaces;
using Thesaurus.core.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database path and register DbContext with SQLite
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "thesaurus.db");
builder.Services.AddDbContext<ThesaurusDbContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}"));

// Register application services
builder.Services.AddScoped<IThesaurus, ThesaurusServiceEf>();

// Add essential services for controllers and API documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(); // Uncomment to enable Swagger documentation 

var app = builder.Build();

// Uncomment the following lines to enable Swagger in development mode
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

// Ensure the database is created and migrations are applied at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ThesaurusDbContext>();
    db.Database.Migrate(); // Applies any pending migrations (only for development, for production use CI/CD)
}

// Configure middleware
app.UseHttpsRedirection();
app.MapControllers(); 
app.Run(); 
