using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface SupportAccountService
{
	Task<IActionResult> ChangePassword(string oldPassword, string newPassword);
	Task<IActionResult> ForgotPassword(string email);
	Task<IActionResult> VerifySecurityCode(ForgotPasswordRequest forgotPasswordRequest);
	Task<IActionResult> ChangeForgotPassword(NewPasswordRequest newPasswordRequest);
	
}
