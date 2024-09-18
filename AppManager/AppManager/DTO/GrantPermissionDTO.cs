using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class GrantPermissionDTO
{
	[Required]
	public string? Module { get; set; }
	[Required]
	public string? Permission { get; set; }
}
