using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface AuthService
{
	Task<IActionResult> Login(UserAccessorDTO userAccessorDTO);
	Task<bool> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
	Task<string> GenerrateJwt(Guid id, string role);
	Task<bool> RevokeToken(Guid id);
}
