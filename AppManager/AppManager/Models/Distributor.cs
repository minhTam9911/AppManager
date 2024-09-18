using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2_Web_API.Models;
 
public partial class Distributor
{
	[Key]
	public Guid Id { get; set; }
	[Required]
	public string? Name { get; set; }
	[Required]
	public string? Email { get; set; }
	[Required]
	public string? Password {  get; set; }
	public string? PhoneNumber { get; set; }
	public string? SecurityCode { get; set; }
	[Required]
	public string? Address { get; set; }
	public Guid? SaleManagementId {  get; set; }
	public Guid? SalesId {  get; set; }
	public bool? IsStatus { get; set; }
	public Guid? CreateBy { get; set; }
	public string? PhotoAvatar { get; set; }

	public string? PasswordResetToken { get; set; }
	public DateTime? ResetTokenExpires { get; set; }

	[Required]
	public int? PositionId { get; set; }
	public DateTime CreatedDate { get; set; }
	[ForeignKey("PositionId")]
	public virtual Position? Position { get; set; }
	[ForeignKey("CreateBy")]
	public virtual StaffUser? StaffUser { get; set; }
	[ForeignKey("SaleManagementId")]
	public virtual StaffUser? SaleManagement { get; set; }
	[ForeignKey("SalesId")]
	public virtual StaffUser? Sales { get; set; }
	public virtual Area? Area { get; set; }
	public virtual ICollection<GrantPermission> GrantPermissions { get; set; } = new List<GrantPermission>();
}
