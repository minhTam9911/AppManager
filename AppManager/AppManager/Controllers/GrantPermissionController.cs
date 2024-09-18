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
public class GrantPermissionController : ControllerBase
{

	private readonly GrantPermissionService grantPermissionService;
	private readonly UserServiceAccessor userServiceAccessor;
	public GrantPermissionController(GrantPermissionService grantPermissionService, UserServiceAccessor userServiceAccessor)
	{
		this.grantPermissionService = grantPermissionService;
		this.userServiceAccessor = userServiceAccessor;
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	public async Task<IActionResult> GetAll()
	{
		try
		{
			return Ok(await grantPermissionService.FindAll());
		}catch(Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-by-id/{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		try
		{
			return Ok(await grantPermissionService.FindById(id));
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-by-name/{name}")]
	public async Task<IActionResult> GetByName(string name)
	{
		try
		{
			
			return Ok(await grantPermissionService.FindByName(name));
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	[Authorize(Roles = "Administrator,Owner")]
	public async Task<IActionResult> Create([FromBody]GrantPermissionDTO request)
	{
		
		return await grantPermissionService.Create(request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("update/{id}")]
	[Authorize(Roles = "Administrator,Owner")]
	public async Task<IActionResult> Put(int id, [FromBody] GrantPermissionDTO request)
	{
		
		return await grantPermissionService.Update(id, request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete/{id}")]
	[Authorize(Roles = "Administrator,Owner")]
	public async Task<IActionResult> Delete(int id)
	{
		
		return await grantPermissionService.Delete(id);
	}
}
