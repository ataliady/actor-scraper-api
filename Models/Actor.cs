using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebActorScraper.Models
{
	public class Actor
	{
		//[Key] // Mark this as the primary key
		//[JsonIgnore]
		public string? Id { get; set; }

		public string? Name { get; set; }
		public string? Details { get; set; }
		public string? Type { get; set; }
		public int Rank { get; set; }
		public string? Provider { get; set; }
	}
}
