using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_API.Models;

namespace Project_2_Web_Api.Service;

public interface PositionGroupService
{

	Task<IActionResult> Create(PositionGroupDTO positionGroupDTO);
	Task<IActionResult> Update(string id,PositionGroupDTO positionGroupDTO);
	Task<IActionResult> Delete(string id);
	Task<dynamic> FindAll();
	Task<dynamic> FindById(string id);
	Task<dynamic> FindByName(string name);

}
