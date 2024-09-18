using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;
using Project_2_Web_API.Models;



namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
[Authorize(Roles = "Administrator,Owner")]
public class PositionGroupController : ControllerBase
{

	private readonly PositionGroupService positionGroupService;
	private readonly UserServiceAccessor userServiceAccessor;
	public PositionGroupController(PositionGroupService positionGroupService, UserServiceAccessor userServiceAccessor)
	{
		this.positionGroupService = positionGroupService;
		this.userServiceAccessor = userServiceAccessor;
	}


	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	public  async Task<IActionResult> GetAll()
	{
		try
		{
			
			return  Ok( await positionGroupService.FindAll());
		}
		catch(Exception ex)
		{
			return BadRequest(new { msg = ex.Message });
		}
		
	}


	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-by-id/{id}")]
	public async Task<IActionResult> GetById(string id)
	{
		try
		{
			
			return Ok(await positionGroupService.FindById(id));
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
			return Ok(await positionGroupService.FindByName(name));
		}
		catch (Exception ex)
		{
			return BadRequest(new { msg = ex.Message });
		}
	}

	
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromBody]PositionGroupDTO request)
	{
		
		return await positionGroupService.Create(request);
	}
	

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("update/{id}")]
	public async Task<IActionResult> Update(string id, [FromBody] PositionGroupDTO request)
	{
		
		return await positionGroupService.Update(id, request);
	}

	

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(string id)
	{
		
		return await positionGroupService.Delete(id);
	}
}
