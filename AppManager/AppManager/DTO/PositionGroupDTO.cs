using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class PositionGroupDTO
{
	[Required]
	public string? Name { get; set; }
}
