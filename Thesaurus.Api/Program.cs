using Thesaurus.Core.Data;
using Microsoft.EntityFrameworkCore;
using Thesaurus.core.Interfaces;
using Thesaurus.core.Services;

var builder = WebApplication.CreateBuilder(args);

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "thesaurus.db");
builder.Services.AddDbContext<ThesaurusDbContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IThesaurus, ThesaurusServiceEf>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ThesaurusDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

