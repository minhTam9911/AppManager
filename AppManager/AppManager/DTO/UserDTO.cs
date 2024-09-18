using Project_2_Web_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class UserDTO
{
	[Required]
	public string? FullName { get; set; }
	public string? PhoneNumber { get; set; }
	public string? Address { get; set; }
	[Required]
	[EmailAddress]
	public string? Email { get; set; }
	public bool? IsStatus { get; set; }
	[Required]
	public int? PositionId { get; set; }
}
