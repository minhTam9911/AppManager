namespace Project_2_Web_Api.DTO;

public class ForgotPasswordRequest
{
	public string? Token { get; set; }
	public string? Code { get; set; }
}
public class NewPasswordRequest
{
	public string? Token { get; set; }
	public string? NewPassword { get; set; }
	public string? ConfirmPassword { get; set; }

}
