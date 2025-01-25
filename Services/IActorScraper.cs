using WebActorScraper.Models;

namespace WebActorScraper.Services
{
	public interface IActorScraper
	{
		List<Actor> ScrapeActors();
	}
}
