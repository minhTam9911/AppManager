
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;
using Project_2_Web_API.Models;
using System.Net;
using System.Security.Claims;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AreaController : ControllerBase
{
	private readonly AreaService areaService;
	private readonly UserServiceAccessor userServiceAccessor;
	public AreaController(AreaService areaService, UserServiceAccessor userServiceAccessor)
	{
		this.areaService = areaService;
		this.userServiceAccessor = userServiceAccessor;
	}


	#region
	/*	[HttpGet("demo")]
		[Authorize]
		public IActionResult demo()
		{
			return Ok(new { id = Guid.NewGuid(), password = BCrypt.Net.BCrypt.HashPassword("abc123"), date = DateTime.Now, token = User?.FindFirstValue(ClaimTypes.Role)
		});
		}*/
	#endregion




	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	public async Task<IActionResult> GetAll()
	{
		try
		{
			
			return Ok(await areaService.FindAll());
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
			
			return Ok(await areaService.FindById(id));
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
			
			return Ok(await areaService.FindByName(name));
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("detail-staff-in-area/{id}")]
	public async Task<IActionResult> DetailStaffInAre(int id)
	{
		try
		{
			return await areaService.DetailStaffInAre(id);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromBody] AreaDTO request)
	{
		
		return await areaService.Create(request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("add-staff-to-area/{idArea}")]
	public async Task<IActionResult> AddStaffToArea(int idArea, [FromQuery(Name = "id-staff")] string idStaff )
	{
		if (await userServiceAccessor.IsGuest())
		{
			return Unauthorized();
		}
		if (await userServiceAccessor.IsDistributor())
		{
			return Unauthorized();
		}
		if (await userServiceAccessor.IsSales())
		{
			return Unauthorized();
		}
		return await areaService.AddStaffToArea(idArea,idStaff);
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("remove-staff-in-area/{idArea}")]
	public async Task<IActionResult> RemoveStaffInArea(int idArea, [FromQuery(Name = "id-staff")] string idStaff)
	{
		if (await userServiceAccessor.IsGuest())
		{
			return Unauthorized();
		}
		if (await userServiceAccessor.IsDistributor())
		{
			return Unauthorized();
		}
		if (await userServiceAccessor.IsSales())
		{
			return Unauthorized();
		}
		return await areaService.RemoveStaffInArea(idArea, idStaff);
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("add-distributor-to-area/{idArea}")]
	public async Task<IActionResult> AddDistributorToArea(int idArea, [FromQuery(Name = "id-ditributor")] string idDistributor)
	{
		if (await userServiceAccessor.IsGuest())
		{
			return Unauthorized();
		}
		if (await userServiceAccessor.IsDistributor())
		{
			return Unauthorized();
		}
		if (await userServiceAccessor.IsSales())
		{
			return Unauthorized();
		}
		return await areaService.AddDistributorToArea(idArea, idDistributor);
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("remove-distributor-in-area/{idArea}")]
	public async Task<IActionResult> RemoveDistributorInArea(int idArea, [FromQuery(Name = "id-ditributor")] string idDistributor)
	{
		if (await userServiceAccessor.IsGuest())
		{
			return Unauthorized();
		}
		if (await userServiceAccessor.IsDistributor())
		{
			return Unauthorized();
		}
		if(await userServiceAccessor.IsSales())
		{
			return Unauthorized();
		}
		return await areaService.RemoveDistributorInArea(idArea, idDistributor);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("update/{id}")]
	public async Task<IActionResult> Update(int id, [FromQuery(Name = "name-area")] string nameArea)
	{
		return await areaService.Update(id, nameArea);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		return await areaService.Delete(id);
	}
}
