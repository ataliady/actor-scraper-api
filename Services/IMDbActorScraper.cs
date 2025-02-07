using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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

			var options = new ChromeOptions();
			options.AddArgument("--headless");
			options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
			IWebDriver driver = new ChromeDriver(options);
			driver.Navigate().GoToUrl(url);
			string pageSource = driver.PageSource;
			driver.Quit();

			HtmlDocument document = new HtmlDocument();
			document.LoadHtml(pageSource);	
			var nodes = document.DocumentNode.SelectNodes("//li[@class='ipc-metadata-list-summary-item']");

			#region Raw html
			//var web = new HtmlWeb();
			//var document = web.Load(url);
			//var rawHtml = document.DocumentNode.OuterHtml;
			//System.IO.File.WriteAllText("debug.html", rawHtml);
			#endregion

			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					var name = node.SelectSingleNode(".//h3")?.InnerText.Trim();
					var details = node.SelectSingleNode(".//div[@data-testid='dli-bio']")?.InnerText.Trim() ?? "";
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
