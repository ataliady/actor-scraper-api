using WebActorScraper.Services;

namespace WebActorScraper.Utilities
{
	public class DataInitializer
	{
		public static void PreloadActors(ActorsDbContext dbContext, IActorScraper scraper)
		{

			if (!dbContext.Actors.Any())
			{
				var actors = scraper.ScrapeActors();
				dbContext.Actors.AddRange(actors);
				dbContext.SaveChanges();
			}
		}
	}
}
