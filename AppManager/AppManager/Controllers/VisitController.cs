using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VisitController : ControllerBase
{

	private readonly VisitService visitService;
	private readonly UserServiceAccessor userServiceAccessor;
	public VisitController(VisitService visitService, UserServiceAccessor userServiceAccessor)
	{
		this.visitService = visitService;
		this.userServiceAccessor = userServiceAccessor;
	}


	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	public async Task<IActionResult> GetAll()
	{
		return await visitService.FindAll();
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all-for-me")]
	public async Task<IActionResult> GetAllForMe()
	{
		return await visitService.FindAllForMe();
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-by-id/{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		return await visitService.FindById(id);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("search")]
	public async Task<IActionResult> Search([FromQuery(Name = "startDate")] DateTime? startDate, [FromQuery(Name = "endDate")] DateTime? endDate, [FromQuery(Name = "staus")] string? status, [FromQuery(Name = "distributorId")] Guid? distributorId)
	{
		return await visitService.Search(startDate,endDate,status,distributorId);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromBody]  VisitDTO request)
	{
		return await visitService.Create(request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("detail/{id}")]
	public async Task<IActionResult> Detail(int id)
	{
		return await visitService.Detail(id);
	}


	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		return await visitService.Delete(id);
	}


	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("history-visiting-schedule")]
	public async Task<IActionResult> HistoryVisitingSchedule()
	{
		return await visitService.HistoryVisitingSchedule();
	}
}
