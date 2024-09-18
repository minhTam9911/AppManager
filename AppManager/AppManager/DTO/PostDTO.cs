using Project_2_Web_API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class PostDTO
{
	[Required]
	public string? Title { get; set; }
	[Required]
	public string? ShortDescription { get; set; }
	[Required]
	public string? PathOfTheArticle { get; set; }
}
