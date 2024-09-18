using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface StaffUserService
{
	Task<IActionResult> Create(StaffUserDTO staffUserDto);
	Task<IActionResult> Update(string id, StaffUserDTO staffUserDto);
	Task<IActionResult> Delete(string id);
	Task<dynamic> FindAll();
	Task<dynamic> FindById(string id);
	Task<dynamic> FindByName(string name);
	Task<IActionResult> ResetPassword(string id);
	Task<IActionResult> SettingPermission(string id, int[] permissions);
}
