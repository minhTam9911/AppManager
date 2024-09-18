using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface VisitService
{
	Task<IActionResult> Create(VisitDTO visitDTO);
	Task<IActionResult> History();
	Task<IActionResult> HistoryVisitingSchedule();
	Task<IActionResult> Delete(int id);
	Task<IActionResult> Search(DateTime? startDate, DateTime? endDate, string? status, Guid? distributorId);
	Task<IActionResult> FindById(int id);
	Task<IActionResult> FindAll();
	Task<IActionResult> FindAllForMe();
	Task<IActionResult> Detail(int id);
}
