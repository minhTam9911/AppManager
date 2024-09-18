using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Project_2_Web_API.Models;

public partial class Visit
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	[Required]
	public DateTime? Calendar {  get; set; }
	[Required]
	public string? Time {  get; set; }

	public Guid? DistributorId { get; set; }

	public Guid? GuestOfVisitId { get; set; }
	
	public Guid? CreateBy {  get; set; }
	[Required]
	public string? PurposeOfVisit {  get; set; }
	
	public string? Status { get; set; }
	[ForeignKey("DistributorId")]	
	public virtual Distributor? Distributor { get; set; }
	[ForeignKey("GuestOfVisitId")]
	public virtual StaffUser? GuestOfVisit { get; set; }
	[ForeignKey("CreateBy")]
	public virtual StaffUser? StaffUser { get; set; }

}
