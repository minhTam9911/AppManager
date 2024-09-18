using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class PositionDTO
{
	[Required]
	public string? Name { get; set; }
	[Range(1,int.MaxValue)]
	public int? PositionGroupId { get; set; }
}
