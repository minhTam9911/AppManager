using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DistributorController : ControllerBase
{
	private readonly DistributorService distributorService;
	private readonly UserServiceAccessor userServiceAccessor;
	public DistributorController(DistributorService distributorService, UserServiceAccessor userServiceAccessor)
	{
		this.distributorService = distributorService;
		this.userServiceAccessor = userServiceAccessor;
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	[Authorize(Roles = "Administrator,Owner")]
	public async Task<IActionResult> GetAll()
	{
		try
		{
			
			return Ok(await distributorService.FindAll());
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-by-id/{id}")]
	public async Task<IActionResult> GetById(string id)
	{
		try
		{
			
			return Ok(await distributorService.FindById(id));
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-by-name/{name}")]
	[Authorize(Roles = "Administrator,Owner")]
	public async Task<IActionResult> GetByName(string name)
	{
		try
		{
			
			return Ok(await distributorService.FindByName(name));
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromBody] DistributorDTO request)
	{
		
		return await distributorService.Create(request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("update/{id}")]
	public async Task<IActionResult> Update(string id, [FromBody] DistributorDTO request)
	{
		
		return await distributorService.Update(id, request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(string id)
	{
		return await distributorService.Delete(id);
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("setting-permission")]
	public async Task<IActionResult> SettingPermission([FromBody] SettingPermissionRequest request)
	{
		try
		{
			return await distributorService.SettingPermission(request.Id, request.PermissionId);
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}

	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("reset-password")]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
	{
		
		return await distributorService.ResetPassword(request.Id);
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("get-me")]
	public async Task<IActionResult> GetMe()
	{
		try
		{ 
			if(await userServiceAccessor.IsDistributor())
			{
				return Ok(await userServiceAccessor.GetByMe());
			}
			else
			{
				return Unauthorized();
			}
			
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
}
