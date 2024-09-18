using Project_2_Web_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2_Web_Api.Models;

public class Notification
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	public string? Title { get; set; }

	public string? Content {  get; set; }

	public Guid? CreateBy {  get; set; }

	public virtual ICollection<StaffUser>? StaffUsers { get; set; } = new List<StaffUser>();
	public virtual ICollection<User>? User { get; set; } = new List<User>();
	public virtual ICollection<Distributor>? Distributors { get; set; } = new List<Distributor>();
}
