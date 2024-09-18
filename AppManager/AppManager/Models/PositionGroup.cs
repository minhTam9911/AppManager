using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2_Web_API.Models;

public partial class PositionGroup
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	[Required]
	public string? Name { get; set; }

	public DateTime? Created { get; set; }

	public virtual ICollection<Position> Positions { get; set; } = new List<Position>();

	
}
