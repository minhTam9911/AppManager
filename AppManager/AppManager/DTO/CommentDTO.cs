using Project_2_Web_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_2_Web_Api.DTO;

public class CommentDTO
{
	[Required]
	public int TaskId { get; set; }

	public int? ParentCommentId { get; set; }
	[Required]
	public string Comment1 { get; set; } = null!;
}
