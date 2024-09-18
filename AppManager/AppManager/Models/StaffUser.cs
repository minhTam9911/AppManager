using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2_Web_API.Models;

public partial class StaffUser
{
	[Key]
	public Guid Id { get; set; }
	[Required]
	public string? Fullname { get; set; }
	[Required]
	public string? Email { get; set; }
	[Required]
	public string? Password { get; set; }
	public string? PhoneNumber { get; set; }
	public string? SecurityCode { get; set; }
	public string? Address { get; set; }
	public string? CreateBy { get; set; }
	public string? PhotoAvatar { get; set; }
	[Required]
	public int? PositionId { get; set; }
	public DateTime CreatedDate { get; set; }
	[ForeignKey("PositionId")]
	public virtual Position? Position { get; set; }
	public string? PasswordResetToken { get; set; }
	public DateTime? ResetTokenExpires { get; set; }
	public bool? IsStatus { get; set; }
	public virtual Area? Area { get; set; }	
	public virtual ICollection<StaffUser>? StaffSuperior { get; set; } = new List<StaffUser>();
	public virtual ICollection<StaffUser>? StaffInterior { get; set; } = new List<StaffUser>();
	public virtual ICollection<GrantPermission> GrantPermissions { get; set; } = new List<GrantPermission>();




}
