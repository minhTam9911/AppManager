using System.ComponentModel.DataAnnotations;

namespace Project_2_Web_Api.DTO;

public class TaskForVisitDTO
{
	[Required]
	public int? VisitId { get; set; }
	[Required]
	public string? Title { get; set; }
	[Required]
	public Guid? AssignedStaffUserId { get; set; }
	[Required]
	public Guid? ReportingStaffUserId { get; set; }
	[Required]
	public string? Description { get; set; }
	[Required]
	public DateTime? StartDate { get; set; }
	[Required]
	public DateTime? EndDate { get; set; }
}
