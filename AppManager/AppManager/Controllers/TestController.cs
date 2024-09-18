using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;
using System.Xml.Linq;

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
	private readonly PositionGroupService positionGroupService;
	private readonly UserServiceAccessor userServiceAccessor;
	private readonly PositionService positionService;
	private readonly StaffUserService staffUserService;
	private static bool _isRun = false;
	private static bool _isRun1 = false;
	private static bool _isRun2 = false;
	public TestController(PositionGroupService positionGroupService, UserServiceAccessor userServiceAccessor, PositionService positionService, StaffUserService staffUserService)
	{
		this.positionGroupService = positionGroupService;
		this.userServiceAccessor = userServiceAccessor;
		this.positionService = positionService;
		this.staffUserService = staffUserService;
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("test0")]
	public async Task<IActionResult> Test()
	{
		if (!_isRun)
		{
			_isRun = true;
			var data = new PositionGroupDTO { Name = "System" };
			return await positionGroupService.Create(data);
		}
		else
		{
			return Unauthorized();
		}

	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("test1")]
	public async Task<IActionResult> Test1()
	{
		if (!_isRun1)
		{
			var data = new PositionDTO { Name = "Administrator", PositionGroupId = 1 };
			_isRun1 = true;
			return await positionService.Create(data);
		}
		return Unauthorized();

	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("test2/{email}")]
	public async Task<IActionResult> Test2(string email)
	{
		if (!_isRun2)
		{
			var data = new StaffUserDTO { Fullname = "Supper admin", Email = email, PositionId = 1, IsStatus = false, StaffSuperiorId = null, StaffInteriorId = null };
			_isRun2 = true;
			return await staffUserService.Create(data);
		}
		return Unauthorized();
	}
	
}
