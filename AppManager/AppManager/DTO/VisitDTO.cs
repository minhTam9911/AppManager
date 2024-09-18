using Project_2_Web_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class VisitDTO
{
	
	[Required]
	public DateTime? Calendar { get; set; }
	[Required]
	public string? Time { get; set; }
	[Required]
	public Guid? DistributorId { get; set; }
	[Required]
	public string? PurposeOfVisit { get; set; }
	public Guid? GuestOfVisitId { get; set; }
	public string? Status { get; set; }
}
