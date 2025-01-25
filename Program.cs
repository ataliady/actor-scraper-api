using WebActorScraper.Models;
using WebActorScraper.Services;
using WebActorScraper.Utilities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure in-memory database
builder.Services.AddDbContext<ActorsDbContext>(options => options.UseInMemoryDatabase("ActorsDB"));

// Register scraper services with dependency injection
builder.Services.AddScoped<IActorScraper, IMDbActorScraper>();
builder.Services.AddScoped<TheNumbersActorScraper>();
builder.Services.AddScoped<Func<string, IActorScraper>>(serviceProvider => providerName =>
{
	return providerName.ToLower() switch
	{
		"imdb" => serviceProvider.GetRequiredService<IActorScraper>(),
		"thenumbers" => serviceProvider.GetRequiredService<TheNumbersActorScraper>(),
		_ => throw new ArgumentException($"Scraper for provider '{providerName}' is not registered.")
	};
});

var app = builder.Build();

// Preload actor data
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<ActorsDbContext>();
//    var scraper = scope.ServiceProvider.GetRequiredService<IActorScraper>();
//    DataInitializer.PreloadActors(dbContext, scraper);
//}

// Preload actor data
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<ActorsDbContext>();
	var scraperFactory = scope.ServiceProvider.GetRequiredService<Func<string, IActorScraper>>();

	// Use the default scraper (IMDb) to preload data
	var defaultScraper = scraperFactory("imdb");
	DataInitializer.PreloadActors(dbContext, defaultScraper);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public class ActorsDbContext : DbContext
{
	public DbSet<Actor> Actors { get; set; }
	public ActorsDbContext(DbContextOptions<ActorsDbContext> options) : base(options) { }
}