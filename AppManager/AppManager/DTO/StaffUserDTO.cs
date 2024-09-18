using Project_2_Web_API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class StaffUserDTO
{
	[Required]
	public string? Fullname { get; set; }
	[Required]
	[EmailAddress]
	public string? Email { get; set; }
	[Required]
	public int? PositionId { get; set; }
	[Required]
	public string[]? StaffSuperiorId { get; set;}
	public string[]? StaffInteriorId { get; set; }
	[Required]
	public bool? IsStatus { get; set; }

}
