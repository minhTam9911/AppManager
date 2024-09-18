using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Helplers;
using Project_2_Web_API.Models;
using System.Net.Mail;
using System.Security.Cryptography;

namespace Project_2_Web_Api.Service.Impl;

public class SupportAccountServiceImpl : SupportAccountService
{

	private DatabaseContext db;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IMapper mapper;
	private readonly IConfiguration configuration;
	private readonly UserServiceAccessor userServiceAccessor;
	private static bool checkVerify = false;
	public SupportAccountServiceImpl(DatabaseContext db,IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserServiceAccessor userServiceAccessor)
	{
		this.configuration = configuration;
		this.mapper = mapper;
		_httpContextAccessor = httpContextAccessor;
		this.db = db;
		this.userServiceAccessor = userServiceAccessor;
	}

	public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
	{
		try
		{

			if (await userServiceAccessor.IsGuest())
			{
				var data = await db.Users.FindAsync(await userServiceAccessor.GetById());
				if (!BCrypt.Net.BCrypt.Verify(oldPassword, data.Password))
				{
					return new BadRequestObjectResult(new { msg = "The old password does not match the new password!!" });
				}
				var hashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
				data.Password = hashPassword;
				db.Entry(data).State = EntityState.Modified;
				if (await db.SaveChangesAsync() > 0)
				{
					return new OkObjectResult(new { msg = true });
				}
				else
				{
					return new BadRequestObjectResult(new { msg = false });
				}
			}
			else if (await userServiceAccessor.IsDistributor())
			{
				var data = await db.Distributors.FindAsync(await userServiceAccessor.GetById());
				if (!BCrypt.Net.BCrypt.Verify(oldPassword, data.Password))
				{
					return new BadRequestObjectResult(new { msg = "The old password does not match the new password!!" });
				}
				var hashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
				data.Password = hashPassword;
				db.Entry(data).State = EntityState.Modified;
				if (await db.SaveChangesAsync() > 0)
				{
					return new OkObjectResult(new { msg = true });
				}
				else
				{
					return new BadRequestObjectResult(new { msg = false });
				}
			}
			else
			{
				var data = await db.StaffUsers.FindAsync( await userServiceAccessor.GetById());
				if (!BCrypt.Net.BCrypt.Verify(oldPassword, data.Password))
				{
					return new BadRequestObjectResult(new { msg = "The old password does not match the new password!!" });
				}
				var hashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
				data.Password = hashPassword;
				db.Entry(data).State = EntityState.Modified;
				if (await db.SaveChangesAsync() > 0)
				{
					return new OkObjectResult(new { msg = true });
				}
				else
				{
					return new BadRequestObjectResult(new { msg = false });
				}
			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> ForgotPassword(string? email)
	{
		try
		{
			if (email == null)
			{
				return new BadRequestObjectResult(new { msg = "Email cannot be empty !!" });
			}
			if (!CheckEmail(email))
			{
				return new BadRequestObjectResult(new { msg = "Email invalidate" });
			}
			var security = RandomHelper.RandomInt(6);
			string? token = null;
			if (await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email) != null)
			{
				var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email); 
				user.PasswordResetToken = RandomToken();
				user.ResetTokenExpires = DateTime.Now.AddHours(1);
				user.SecurityCode = security;
				db.Users.Update(user);
				if (await db.SaveChangesAsync() > 0)
				{
					token = user.PasswordResetToken;
				}
				else
				{
					return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
				}
			}
			
			else if (await db.StaffUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email) != null)
			{
				var staff = await db.StaffUsers.FirstOrDefaultAsync(x => x.Email == email);
				staff.SecurityCode = security;
				staff.PasswordResetToken = RandomToken();
				staff.ResetTokenExpires = DateTime.Now.AddHours(1);
				db.StaffUsers.Update(staff);
				if (await db.SaveChangesAsync() > 0)
				{
					token = staff.PasswordResetToken;
				}
				else
				{
					return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
				}
			}
			else if (await db.Distributors.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email) != null)
			{
				var distributor = await db.Distributors.FirstOrDefaultAsync(x => x.Email == email);
				distributor.SecurityCode = security;
				distributor.PasswordResetToken = RandomToken();
				distributor.ResetTokenExpires = DateTime.Now.AddHours(1);
				db.Distributors.Update(distributor);
				if (await db.SaveChangesAsync() > 0)
				{
					token = distributor.PasswordResetToken;
				}
				else
				{
					return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
				}
			}
			else
			{
				return new BadRequestObjectResult(new { msg = "Email does not exist" });
			}
			var mailHelper = new MailHelper(configuration);
			
			var check = mailHelper.Send(configuration["Gmail:Username"], email, "Email verification code", MailHelper.HtmlVerify(security));
			if (!check)
			{
				return new BadRequestObjectResult(new { msg = "Email sending failed." });
			}
			else
			{
				return new OkObjectResult(new { msg = true, token = token });
			}

		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> VerifySecurityCode(ForgotPasswordRequest forgotPasswordRequest)
	{
		try
		{
			if (forgotPasswordRequest.Token.IsNullOrEmpty())
			{
				return new BadRequestObjectResult(new { msg = "token is requried !!" });
			}


			if (await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.PasswordResetToken == forgotPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now) != null)
			{
				var user = await db.Users.FirstOrDefaultAsync(x => x.PasswordResetToken == forgotPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now);
				if (user.SecurityCode == forgotPasswordRequest.Code)
				{
					user.SecurityCode = null;
					db.Users.Update(user);
					if (await db.SaveChangesAsync() > 0) ;
					else
					{
						return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
					}
				}
				else
				{
					
					return new BadRequestObjectResult(new { msg = "Verification code does not match", valid = false });
				}


			}
			else if (await db.StaffUsers.AsNoTracking().FirstOrDefaultAsync(x => x.PasswordResetToken == forgotPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now) != null)
			{
				var staff = await db.StaffUsers.FirstOrDefaultAsync(x => x.PasswordResetToken == forgotPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now);
				
				if (staff.SecurityCode == forgotPasswordRequest.Code)
				{
					staff.SecurityCode = null;
					db.StaffUsers.Update(staff);
					if (await db.SaveChangesAsync() > 0) ;
					else
					{
						return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
					}
				}
				else
				{
					return new BadRequestObjectResult(new { msg = "Verification code does not match", valid = false });
				}
			}
			else if (await db.Distributors.AsNoTracking().FirstOrDefaultAsync(x => x.PasswordResetToken == forgotPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now) != null)
			{
				var distributor = await db.Distributors.FirstOrDefaultAsync(x => x.PasswordResetToken == forgotPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now);
				if (distributor.SecurityCode == forgotPasswordRequest.Code)
				{
					distributor.SecurityCode = null;
					db.Distributors.Update(distributor);
					if (await db.SaveChangesAsync() > 0) ;
					else
					{
						return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
					}
				}
				else
				{
					return new BadRequestObjectResult(new { msg = "Verification code does not match", valid = false });
				}
			}
			else
			{
				return new BadRequestObjectResult(new { msg = "Token Expried" });
			}
			checkVerify = true;
			return new OkObjectResult(new { msg = "Valid verification code!", valid = true });

		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}

	}

	public async Task<IActionResult> ChangeForgotPassword(NewPasswordRequest newPasswordRequest)
	{
		if (checkVerify)
		{
		try
		{
			if (newPasswordRequest.Token.IsNullOrEmpty())
			{
				return new BadRequestObjectResult(new { msg = "TOken is requried!!" });
			}
			var hashPassword = BCrypt.Net.BCrypt.HashPassword(newPasswordRequest.NewPassword);
				
			if (!BCrypt.Net.BCrypt.Verify(newPasswordRequest.ConfirmPassword, hashPassword)){
				return new BadRequestObjectResult(new { msg = "Password and confirm password not match!!" });
			}
			if (await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.PasswordResetToken == newPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now) != null)
			{
					var user = await db.Users.FirstOrDefaultAsync(x => x.PasswordResetToken == newPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now);
					user.Password = hashPassword;
					user.ResetTokenExpires = null;
					user.PasswordResetToken = null;
					db.Users.Update(user);
				if (await db.SaveChangesAsync() > 0) ;
				else
				{
					return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
				}
			}
			else if (await db.StaffUsers.AsNoTracking().FirstOrDefaultAsync(x => x.PasswordResetToken == newPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now) != null)
			{

					var staff = await db.StaffUsers.FirstOrDefaultAsync(x => x.PasswordResetToken == newPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now);
							staff.Password = hashPassword;
							staff.ResetTokenExpires = null;
							staff.PasswordResetToken = null;
							db.StaffUsers.Update(staff);
							if (await db.SaveChangesAsync() > 0) ;
							else
							{
								return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
							}
					
			}
			else if (await db.Distributors.AsNoTracking().FirstOrDefaultAsync(x => x.PasswordResetToken == newPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now) != null)
			{
					var distributor = await db.Distributors.FirstOrDefaultAsync(x => x.PasswordResetToken == newPasswordRequest.Token && x.ResetTokenExpires > DateTime.Now);
					distributor.Password = hashPassword;
				distributor.ResetTokenExpires = null;
				distributor.PasswordResetToken = null;
					db.Distributors.Update(distributor);
					if (await db.SaveChangesAsync() > 0) ;
				else
				{
					return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
				}
			}
			else
			{

				return new BadRequestObjectResult(new { msg = "token expried or invalid" });
			}
				checkVerify = false;
			return new OkObjectResult(new { msg = true });

		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
		}
		else
		{
			return new UnauthorizedResult();
		}
		
	}

	public bool CheckEmail(string email)
	{
		bool valid = true;
		try
		{
			var emailAddress = new MailAddress(email);
		}
		catch (Exception ex)
		{
			return false;
		}
		return valid;
	}
	private string RandomToken()
	{
		return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
	}

	
}
