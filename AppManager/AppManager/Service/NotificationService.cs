using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface NotificationService
{
	Task<IActionResult> Create(NotificationDTO notificationDTO);
	Task<IActionResult> FindAll();
	Task<IActionResult> FindAllForMe();
	Task<IActionResult> Delete(int id);
	void SendAutoNotificationVisit();
}
