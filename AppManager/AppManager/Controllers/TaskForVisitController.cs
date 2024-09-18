using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TaskForVisitController : ControllerBase
{
	private readonly TaskForVisitService taskForVisitService;
	private readonly UserServiceAccessor userServiceAccessor;
	public TaskForVisitController(TaskForVisitService taskForVisitService, UserServiceAccessor userServiceAccessor)
	{
		this.taskForVisitService = taskForVisitService;
		this.userServiceAccessor = userServiceAccessor;
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	public async Task<IActionResult> GetAll()
	{
		return await taskForVisitService.FindAll();
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-by-id/{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		return await taskForVisitService.FindById(id);
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("detail-id/{id}")]
	public async Task<IActionResult> Detail(int id)
	{
		return await taskForVisitService.DetailTask(id);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all-task-for-me")]
	public async Task<IActionResult> TaskForMe()
	{
		return await taskForVisitService.TaskForMe();
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromBody] TaskForVisitDTO request)
	{
		return await taskForVisitService.Create(request);
	}

	[Produces("application/json")]
	[Consumes("multipart/form-data")]
	[HttpPut("upload-all-file/{id}")]
	public async Task<IActionResult> UploadAllFile(int id, IFormFile[] fileAssigned, IFormFile[] fileReport)
	{
		return await taskForVisitService.UploadFile(id,fileAssigned,fileReport);
	}


	[Produces("application/json")]
	[Consumes("multipart/form-data")]
	[HttpPut("upload-report-file/{id}/{status}")]
	public async Task<IActionResult> UploadReportFile(int id, IFormFile[] fileReport, string? status)
	{
		return await taskForVisitService.UploadFileTaskForMe(id, fileReport,status);
	}
}
