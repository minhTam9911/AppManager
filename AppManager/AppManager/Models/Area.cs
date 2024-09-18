using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2_Web_API.Models;

public partial class Area
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	public string? Code { set; get; }
	public string? Name { get; set; }
	public virtual ICollection<StaffUser>? StaffUsers { get; set; } = new List<StaffUser>();
	public virtual ICollection<Distributor>? Distributors { get; set; } = new List<Distributor>();
}
