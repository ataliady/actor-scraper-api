using HtmlAgilityPack;
using System.Text.RegularExpressions;
using WebActorScraper.Models;

namespace WebActorScraper.Services
{
	public class IMDbActorScraper : IActorScraper
	{
		public List<Actor> ScrapeActors()
		{
			string url = "https://www.imdb.com/list/ls054840033/";
			var actors = new List<Actor>();
			var web = new HtmlWeb();
			var document = web.Load(url);
			var nodes = document.DocumentNode.SelectNodes("//li[@class='ipc-metadata-list-summary-item']");

			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					var name = node.SelectSingleNode(".//h3")?.InnerText.Trim();
					var details = node.SelectSingleNode(".//div[@data-testid='dli-item-description']")?.InnerText.Trim();
					var type = node.SelectSingleNode(".//li[@class=\"ipc-inline-list__item sc-7c95f518-5 eXHlql\"][1]")?.InnerText.Trim();

					if (!string.IsNullOrEmpty(name))
					{
						// Use regex to extract the name part
						name = Regex.Replace(name, @"^\d+\.\s*", "");
						actors.Add(new Actor
						{
							Id = Guid.NewGuid().ToString(),
							Name = name,
							Details = details,
							Type = type,
							Rank = actors.Count + 1,
							Provider = "IMDB"
						});
					}
				}
			}
			return actors;
		}

	}
}
