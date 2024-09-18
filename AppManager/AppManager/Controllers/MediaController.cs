using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.Service;

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MediaController : ControllerBase
{
	private readonly MediaService mediaService;
	private readonly UserServiceAccessor userServiceAccessor;
	public MediaController(MediaService mediaService, UserServiceAccessor userServiceAccessor)
	{
		this.mediaService = mediaService;
		this.userServiceAccessor = userServiceAccessor;
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	public async Task<IActionResult> GetAll()
	{
		try
		{
			return await mediaService.FindAll();
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all-for-me")]
	public async Task<IActionResult> GetAllForMe()
	{
		try
		{
			return await mediaService.FindAllForMe();
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("multipart/form-data")]
	[HttpPost("create")]
	public async Task<IActionResult> Create(IFormFile filePath)
	{
		try
		{
			return await mediaService.Create(filePath);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			return await mediaService.Delete(id);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
}
