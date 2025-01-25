using HtmlAgilityPack;
using WebActorScraper.Models;

namespace WebActorScraper.Services
{
	public class TheNumbersActorScraper : IActorScraper
	{
		public List<Actor> ScrapeActors()
		{
			string url = "https://www.the-numbers.com/box-office-star-records/worldwide/lifetime-acting/top-grossing-leading-stars";
			var actors = new List<Actor>();
			var web = new HtmlWeb();
			var document = web.Load(url);
			var nodes = document.DocumentNode.SelectNodes("//div[@id=\"page_filling_chart\"]//tbody//tr");

			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					var name = node.SelectSingleNode(".//a")?.InnerText.Trim();

					if (!string.IsNullOrEmpty(name))
					{
						actors.Add(new Actor
						{
							Id = Guid.NewGuid().ToString(),
							Name = name,
							Details = "",
							Type = "Actor",
							Rank = actors.Count + 1,
							Provider = "TheNumbers"
						});
					}
				}
			}
			return actors;
		}

	}
}
