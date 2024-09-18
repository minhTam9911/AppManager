using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private AuthService authService;
	private readonly IConfiguration configuration;
	public AuthController(AuthService authService, IConfiguration configuration)
	{
		this.authService = authService;
		this.configuration = configuration;
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UserAccessorDTO request)
	{
		return await authService.Login(request);
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("refresh-token")]
	public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
	{

		var principal = GetClaimsPrincipal(request.AccessToken);
		if(principal?.Identity?.Name is null)
		{
			return Unauthorized();
		}
		if(!await authService.RefreshTokenAsync(request))
		{
			return Unauthorized();
		}
		var jwtToken = await authService.GenerrateJwt(Guid.Parse(principal?.Identity?.Name), principal.FindFirstValue(ClaimTypes.Role));
		return Ok(new { accessToken = jwtToken , refreshToken = request.RefreshToken});
	}
	[HttpDelete("revoke")]
	[Authorize]
	public async Task<IActionResult> RevokeToken()
	{
		if(HttpContext.User.Identity.Name.IsNullOrEmpty())
		{
			return Unauthorized();
		}
		if (!await authService.RevokeToken(Guid.Parse(HttpContext.User.Identity.Name)))
		{
			return Unauthorized();
		}
		return Ok();
	}
	private ClaimsPrincipal? GetClaimsPrincipal(string? token)
	{
			var tokenValidation = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				ValidateAudience = false,
				ValidateIssuer = false,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
				configuration.GetSection("AppSettings:Token").Value!)),
				ValidateLifetime = false
			};
			return new JwtSecurityTokenHandler().ValidateToken(token, tokenValidation, out _);
	}
}
