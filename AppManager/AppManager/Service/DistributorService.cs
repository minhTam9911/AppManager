using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface DistributorService
{

	Task<IActionResult> Create(DistributorDTO distributorDTO);
	Task<IActionResult> Update(string id, DistributorDTO distributorDTO);
	Task<IActionResult> Delete(string id);
	Task<dynamic> FindAll();
	Task<dynamic> FindById(string id);
	Task<dynamic> FindByName(string name);
	/*	Task<dynamic> FindByEmailOnly(string name);
		Task<dynamic> FindByEmails(string name);*/
	Task<IActionResult> ResetPassword(string id);
	Task<IActionResult> SettingPermission(string id, int[] permissions);
}
