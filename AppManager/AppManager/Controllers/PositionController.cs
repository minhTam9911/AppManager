using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;
using Project_2_Web_API.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PositionController : ControllerBase
{

	private readonly PositionService positionService;
	private readonly UserServiceAccessor userServiceAccessor;
	public PositionController(PositionService positionService, UserServiceAccessor userServiceAccessor)
	{
		this.positionService = positionService;
		this.userServiceAccessor = userServiceAccessor;
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	public async Task<IActionResult> GetAll()
	{
		try
		{
			
			return Ok(await positionService.FindAll());
		}catch(Exception ex)
		{
			return BadRequest(new { msg = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-by-id/{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		try
		{
			
			return Ok(await positionService.FindById(id));
		}
		catch (Exception ex)
		{
			return BadRequest(new { msg = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-by-name/{name}")]
	public async Task<IActionResult> GetByName(string name)
	{
		try
		{
			
			return Ok(await positionService.FindByName(name));
		}
		catch (Exception ex)
		{
			return BadRequest(new { msg = ex.Message });
		}
	}


	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	[Authorize(Roles = "Administrator,Owner")]
	public async Task<IActionResult> Create([FromBody] PositionDTO request)
	{
		
		return await positionService.Create(request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("update/{id}")]
	[Authorize(Roles = "Administrator,Owner")]
	public async Task<IActionResult> Update(int id, [FromBody] PositionDTO request)
	{
		
		return await positionService.Update(id, request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete/{id}")]
	[Authorize(Roles = "Administrator,Owner")]
	public  async Task<IActionResult> Delete(int id)
	{
		
		return await positionService.Delete(id);
	}
}
