using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class NotificationDTO
{
	[Required]
	public string? Title { get; set; }
	[Required]
	public string? Content { get; set; }
	[Required]
	public List<string>? Receiver { get; set; }
}
