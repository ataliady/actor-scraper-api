using WebActorScraper.Models;
using WebActorScraper.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebActorScraper.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class ActorController(ActorsDbContext dbContext, Func<string, IActorScraper> scraperFactory) : ControllerBase
	{
		private readonly ActorsDbContext _dbContext = dbContext;
		private readonly Func<string, IActorScraper> _scraperFactory = scraperFactory;

		[HttpGet]
		public IActionResult GetAllActors(
			[FromQuery] string? name,
			[FromQuery] int? minRank,
			[FromQuery] int? maxRank,
			[FromQuery] string? provider,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10)
		{

			// Determine the provider (default to "imdb" if none specified)
			string providerName = string.IsNullOrWhiteSpace(provider) ? "imdb" : provider.Replace(" ", "").ToLower();

			// Check if data for the specified provider already exists in the database
			if (!_dbContext.Actors.Any(a => a.Provider.Equals(providerName, StringComparison.OrdinalIgnoreCase)))
			{
				// Use the scraper to fetch and save data for the specified provider
				try
				{
					var scraper = _scraperFactory(providerName);
					var _actors = scraper.ScrapeActors();
					_dbContext.Actors.AddRange(_actors);
					_dbContext.SaveChanges();
				}
				catch (ArgumentException ex)
				{
					return BadRequest(ex.Message);
				}
			}

			var query = _dbContext.Actors.AsQueryable();

			if (!string.IsNullOrWhiteSpace(name))
				query = query.Where(a => a.Name.Contains(name));

			if (minRank.HasValue)
				query = query.Where(a => a.Rank >= minRank);

			if (maxRank.HasValue)
				query = query.Where(a => a.Rank <= maxRank);

			query = query.Where(a => a.Provider.Equals(providerName, StringComparison.OrdinalIgnoreCase));

			var totalActors = query.Count();
			var actors = query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(a => new { a.Id, a.Name })
				.ToList();

			return Ok(new { Total = totalActors, Actors = actors });
		}

		[HttpGet("{id}")]
		public IActionResult GetActorById(string id)
		{
			var actor = _dbContext.Actors.Find(id);
			if (actor == null) return NotFound("Actor not found");

			return Ok(actor);
		}

		[HttpPost]
		public IActionResult AddActor([FromBody] ActorRequest actorRequest)
		{
			if (actorRequest == null) return NotFound("null");
			if (_dbContext.Actors.Any(a => a.Rank == actorRequest.Rank && a.Provider == actorRequest.Provider))
				return BadRequest("Actor with the same rank already exists");

			var actor = new Actor
			{
				Id = Guid.NewGuid().ToString(), // Automatically generate the ID
				Name = actorRequest.Name,
				Details = actorRequest.Details,
				Type = actorRequest.Type,
				Rank = actorRequest.Rank,
				Provider = actorRequest.Provider
			};
			_dbContext.Actors.Add(actor);
			_dbContext.SaveChanges();

			return CreatedAtAction(nameof(GetActorById), new { id = actor.Id }, new { actor.Id, actor.Name, actor.Details, actor.Type, actor.Rank, actor.Provider });
		}

		[HttpPut("{id}")]
		public IActionResult UpdateActor(string id, [FromBody] Actor updatedActor)
		{
			var actor = _dbContext.Actors.Find(id);
			if (actor == null) return NotFound("Actor not found");

			if (_dbContext.Actors.Any(a => a.Rank == updatedActor.Rank && a.Id != updatedActor.Id))
				return BadRequest("Duplicated rank");

			if (!string.IsNullOrEmpty(updatedActor.Name))
				actor.Name = updatedActor.Name;

			if (!string.IsNullOrEmpty(updatedActor.Details))
				actor.Details = updatedActor.Details;

			if (!string.IsNullOrEmpty(updatedActor.Type))
				actor.Type = updatedActor.Type;

			if (!string.IsNullOrEmpty(updatedActor.Provider))
				actor.Provider = updatedActor.Provider;
			
			if (updatedActor.Rank != 0)
				actor.Rank = updatedActor.Rank;

			_dbContext.SaveChanges();
			return NoContent();
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteActor(string id)
		{
			var actor = _dbContext.Actors.Find(id);
			if (actor == null) return NotFound("Actor not found");

			_dbContext.Actors.Remove(actor);
			_dbContext.SaveChanges();
			return NoContent();
		}
	}
}
