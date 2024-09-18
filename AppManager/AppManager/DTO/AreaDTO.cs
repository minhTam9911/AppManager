using Project_2_Web_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class AreaDTO
{
	[Required]
	public string? Code { set; get; }
	[Required]
	public string? Name { get; set; }
}
	
