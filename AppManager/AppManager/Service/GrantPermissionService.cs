using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_API.Models;

namespace Project_2_Web_Api.Service;

public interface GrantPermissionService
{
	Task<IActionResult> Create(GrantPermissionDTO grantPermissionDTO);
	Task<IActionResult> Update(int id, GrantPermissionDTO grantPermissionDTO);
	Task<IActionResult> Delete(int id);
	Task<dynamic> FindAll();
	Task<dynamic> FindById(int id);
	Task<dynamic> FindByName(string name);
}
