using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface TaskForVisitService
{
	Task<IActionResult> Create(TaskForVisitDTO taskForVisitDTO);
	Task<IActionResult> FindAll();
	Task<IActionResult> FindById(int id);
	Task<IActionResult> UploadFile(int id, IFormFile[] fileAssigned, IFormFile[] fileReport);
	Task<IActionResult> TaskForMe();
	Task<IActionResult> DetailTask(int id);
	Task<IActionResult> UploadFileTaskForMe(int id, IFormFile[] fileReport, string status);

}
