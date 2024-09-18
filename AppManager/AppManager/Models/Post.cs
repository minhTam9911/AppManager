using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2_Web_API.Models;

public partial class Post
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[Required]
	public string? Title { get; set; }
	[Required]
	public string? ShortDescription {  get; set; }
	public Guid? CreateBy { get;set; }	
	public DateTime? CreateDate { get; set; }	
	public bool IsStatus { get; set; }
	public string? FilePath { get; set; }
	[Required]
	public string? PathOfTheArticle { get; set; }
	[ForeignKey(nameof(CreateBy))]		
	public virtual StaffUser? StaffUser { get; set; }	

}
