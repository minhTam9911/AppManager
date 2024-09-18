using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class UserAccessorDTO
{
	[Required]
	public string? Username { get; set; }
	[Required]
	public string? Password { get; set; }
}
