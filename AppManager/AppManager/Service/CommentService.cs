using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface CommentService
{
	Task<IActionResult> Create(CommentDTO commentDTO);
	Task<IActionResult> Reply(CommentDTO commentDTO);
	Task<IActionResult> Delete(int id);
	Task<IActionResult> FindAll(int idTask);
}
