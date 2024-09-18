using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SupportAccountController : ControllerBase
{
	private readonly SupportAccountService supportAccountService;
	public SupportAccountController(SupportAccountService supportAccountService)
	{
		this.supportAccountService = supportAccountService;
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("change-password")]
	[Authorize]
	public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
	{
		return await supportAccountService.ChangePassword(request.oldPassword, request.newPassword);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("forgot-password")]
	public async Task<IActionResult> ForgotPassword([FromQuery(Name = "email")] string email)
	{
		
		return await supportAccountService.ForgotPassword(email);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("verify")]
	public async Task<IActionResult> VerifyCode(ForgotPasswordRequest request)
	{
		
		return await supportAccountService.VerifySecurityCode(request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("change-forgot-password")]
	public async Task<IActionResult> ChangeForgotPassword([FromBody] NewPasswordRequest request)
	{
		return await supportAccountService.ChangeForgotPassword(request);
	}

	
}
