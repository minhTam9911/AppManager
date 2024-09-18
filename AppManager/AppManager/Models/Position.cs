using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2_Web_API.Models;

public partial class Position
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[Required]
	public string? Name { get; set; }

	public DateTime? Created { get; set; }
	[Required] 
	public int? PositionGroupId {  get; set; }
	[ForeignKey(nameof(PositionGroupId))]	
	public virtual PositionGroup? PositionGroup { get; set; }

	public virtual ICollection<User> Users { get; set; } = new List<User>();

}
