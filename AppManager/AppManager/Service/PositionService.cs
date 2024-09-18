using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_API.Models;

namespace Project_2_Web_Api.Service;

public interface PositionService
{
	Task<IActionResult> Create(PositionDTO positionDTO);
	Task<IActionResult> Update(int id, PositionDTO positionDTO);
	Task<IActionResult> Delete(int id);
	Task<dynamic> FindAll();
	Task<dynamic> FindById(int id);
	Task<dynamic> FindByName(string name);
}
