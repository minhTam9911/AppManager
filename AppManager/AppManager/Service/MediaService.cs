using Microsoft.AspNetCore.Mvc;

namespace Project_2_Web_Api.Service;

public interface MediaService
{
	Task<IActionResult> Create(IFormFile filePath);
	Task<IActionResult> Delete(int id);
	Task<IActionResult> FindAllForMe();
	Task<IActionResult> FindAll();

}
