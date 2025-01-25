using System.ComponentModel.DataAnnotations;

namespace WebActorScraper.Models
{
	public class ActorRequest
	{
		[Required]
		public string Name { get; set; }

		public string Details { get; set; }
		public string Type { get; set; }
		public int Rank { get; set; }
		public string Provider { get; set; }
	}
}
