using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2_Web_API.Models;

public class TaskForVisit
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	[Required]
	public string? Title { get; set; }
	[Required]
	public int? VisitId { get; set; }
	[Required]
	public  Guid? AssignedStaffUserId {  get; set; }
	[Required]
	public  Guid? ReportingStaffUserId {  get; set; }
	[Required]
	public string? Description { get; set; }
	[Required]
	public DateTime? StartDate { get; set; }
	[Required]
	public DateTime? EndDate {  get; set; }

	public Guid? CreateBy { get; set; }
	
	public string? Status { get; set; }
	public virtual List<Visit>? Visits { get; set; } = new List<Visit>();
	public virtual List<Comment>? Comments { get; set; }
	public virtual ICollection<PhotoPathAssigned>? PhotoPathAssigned { get; set; } = new List<PhotoPathAssigned>();
	public virtual ICollection<PhotoPathReporting>? PhotoPathReporting { get; set; } = new List<PhotoPathReporting>();
	[ForeignKey(nameof(AssignedStaffUserId))]
	public virtual StaffUser? StaffUserAssignee { get; set; }
	[ForeignKey(nameof(ReportingStaffUserId))]
	public virtual StaffUser? StaffUserReposter { get; set; }
	
}

public class PhotoPathAssigned
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	public string? Path { get; set; }
}

public class PhotoPathReporting
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	public string? Path { get; set; }
}