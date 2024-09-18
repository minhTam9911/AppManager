using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface PostService
{
	Task<IActionResult> Create(PostDTO postDTO);
	Task<IActionResult> Upload(int id, IFormFile filePath);
	Task<IActionResult> Update(int id,PostDTO postDTO);
	Task<IActionResult> Search(string keyword);
	Task<IActionResult> UpdateStatus(int id,bool status);
	Task<IActionResult> FindAll();
	Task<IActionResult> FindAllForMe();
	Task<IActionResult> Delete(int id);
	Task<IActionResult> DeleteRange(int[] id);
}
