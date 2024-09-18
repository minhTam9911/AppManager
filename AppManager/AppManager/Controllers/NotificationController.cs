using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationController : ControllerBase
{
	private readonly NotificationService notificationService;
	private readonly UserServiceAccessor userServiceAccessor;
	public NotificationController(NotificationService notificationService, UserServiceAccessor userServiceAccessor)
	{
		this.notificationService = notificationService;
		this.userServiceAccessor = userServiceAccessor;
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	public async Task<IActionResult> GetAll()
	{
		return await notificationService.FindAll();
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all-for-me")]
	public async Task<IActionResult> GetAllForMe()
	{
		return await notificationService.FindAllForMe();
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromBody]NotificationDTO request)
	{
		return await notificationService.Create(request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		return await notificationService.Delete(id);
	}
}
