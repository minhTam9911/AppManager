using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Models;
using Project_2_Web_API.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http.ModelBinding;
using System.Xml.Linq;

namespace Project_2_Web_Api.Service.Impl;

public class AuthServiceImpl : AuthService
{

	private DatabaseContext db;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IMapper mapper;
	private IConfiguration configuration;
	public AuthServiceImpl(DatabaseContext db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper)
	{
		this.mapper = mapper;
		this.configuration = configuration;
		_httpContextAccessor = httpContextAccessor;
		this.db = db;
	}


	public async Task<IActionResult> Login(UserAccessorDTO userAccessorDTO)
	{
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				var data1 = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == userAccessorDTO.Username);
				if ( data1 != null)
				{
					var data = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == userAccessorDTO.Username);
					if (BCrypt.Net.BCrypt.Verify(userAccessorDTO.Password, data.Password)){
						var accessToken = CreateToken(data.Id, data.Position.Name);
						var refreshToken = RefreshToken();
						refreshToken.UserId = data.Id;
						data1 = null;
						data = null;
						if (await db.ApiTokens.FindAsync(refreshToken.UserId) != null)
						{
							db.ApiTokens.Remove(await db.ApiTokens.FindAsync(refreshToken.UserId));
							await db.SaveChangesAsync();
							db.ApiTokens.Add(refreshToken);
							if (await db.SaveChangesAsync() > 0)
							{
								return new OkObjectResult(new
								{
									accessToken = accessToken,
									refreshToken = refreshToken.RefreshToken
								});
							}
							else
							{
								return new BadRequestObjectResult(new { msg = "System Error !" });
							}

						}
						else
						{
							db.ApiTokens.Add(refreshToken);
							if (await db.SaveChangesAsync() > 0)
							{
							return new OkObjectResult(new
							{
								accessToken = accessToken,
								refreshToken = refreshToken.RefreshToken
							});
							}
							else
							{
							return new BadRequestObjectResult(new { error = "System Error !" });
							}
						}
						
					}
					else
					{
						return new BadRequestObjectResult(new {msg = "Email or Passwords is invalid" });
					}

				}
				var data2 = await db.StaffUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Email == userAccessorDTO.Username);
				 if ( data2!= null)
				{
					if (BCrypt.Net.BCrypt.Verify(userAccessorDTO.Password, data2.Password))
					{
						var position = await db.Positions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data2.PositionId);
						var accessToken = CreateToken(data2.Id, position.Name);
						var refreshToken = RefreshToken();
						refreshToken.UserId = data2.Id;
						data2 = null;

						if (await db.ApiTokens.FindAsync(refreshToken.UserId) != null)
						{

							db.ApiTokens.Remove(await db.ApiTokens.FindAsync(refreshToken.UserId));
							await db.SaveChangesAsync();
							db.ApiTokens.Add(refreshToken);
							if (await db.SaveChangesAsync() > 0)
							{
								return new OkObjectResult(new
								{
									accessToken = accessToken,
									refreshToken = refreshToken.RefreshToken
								});
							}
							else
							{
								return new BadRequestObjectResult(new { msg = "System Error !" });
							}

						}
						else
						{


							db.ApiTokens.Add(refreshToken);
							if (await db.SaveChangesAsync() > 0)
							{
								return new OkObjectResult(new
								{
									accessToken = accessToken,
									refreshToken = refreshToken.RefreshToken
								});
							}
							else
							{
								return new BadRequestObjectResult(new { msg = "System Error !" });
							}
						}
					}
					else
					{
						return new BadRequestObjectResult(new { msg = "Email or Passwords is invalid" });
					}
				}
				var data3 = await db.Distributors.AsNoTracking().FirstOrDefaultAsync(x => x.Email == userAccessorDTO.Username);
				 if (data3!= null)
				{
					if (BCrypt.Net.BCrypt.Verify(userAccessorDTO.Password, data3.Password))
					{
						var position = await db.Positions.AsNoTracking().FirstOrDefaultAsync(x=>x.Id ==data3.PositionId);
						var accessToken = CreateToken(data3.Id, position.Name);
						var refreshToken = RefreshToken();
						refreshToken.UserId = data3.Id;
						data3 = null;
						if (await db.ApiTokens.FindAsync(refreshToken.UserId) != null)
						{
							db.ApiTokens.Remove(await db.ApiTokens.FindAsync(refreshToken.UserId));
							await db.SaveChangesAsync();
							db.ApiTokens.Add(refreshToken);
							if (await db.SaveChangesAsync() > 0)
							{
								return new OkObjectResult(new
								{
									accessToken = accessToken,
									refreshToken = refreshToken.RefreshToken
								});
							}
							else
							{
								return new BadRequestObjectResult(new { msg = "System Error !" });
							}
						}
						else
						{
							db.ApiTokens.Add(refreshToken);
							if (await db.SaveChangesAsync() > 0)
							{
								return new OkObjectResult(new
								{
									accessToken = accessToken,
									refreshToken = refreshToken.RefreshToken
								});
							}
							else
							{
								return new BadRequestObjectResult(new { msg = "System Error !" });
							}
						}
					}
					else
					{
						return new BadRequestObjectResult(new { msg = "Email or Passwords is invalid" });
					}
				}
				else
				{
					return new BadRequestObjectResult(new { msg = "Username not found" });
				}
			}
		}
		catch(Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}
	private string CreateToken(Guid id, string role)
	{
		List<Claim> claims = new List<Claim> { 
		
			new Claim(ClaimTypes.Name, id.ToString()),
			new Claim(ClaimTypes.Role, role)

		};
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
			configuration.GetSection("AppSettings:Token").Value!));
		//Debug.WriteLine(configuration.GetSection("AppSettings:Token").Value!);
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
		var token = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.Now.AddHours(1),
			signingCredentials:creds

			) ;

		var jwt = new JwtSecurityTokenHandler().WriteToken(token);

		return jwt;
	}

	private ApiToken RefreshToken()
	{
		var expires = DateTime.Now.AddDays(7);
		var more = Guid.NewGuid().ToString().Replace("-", "");
		var token = more + Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		var cookieOption = new CookieOptions
		{
			HttpOnly = true,
			Expires = expires,

		};
		
		_httpContextAccessor.HttpContext.Response.Cookies.Append("resfreshToken", token, cookieOption);
		var apiToken = new ApiToken
		{
			RefreshToken = token,
			Exipres = expires
		};
		return apiToken;
	}

	public string GetMe()
	{
		var result = string.Empty;
		if(_httpContextAccessor.HttpContext != null)
		{
			result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
		}
		return result;
	}

	public async Task<bool> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
	{
		var check = await db.ApiTokens.FirstOrDefaultAsync(x => x.RefreshToken == refreshTokenRequest.RefreshToken);
		if (check != null && check.Exipres > DateTime.Now)
		{
			return true;

			#region
			/*if (await db.Users.FindAsync(check.UserId) != null)
			{
				var database = await db.Users.FindAsync(check.UserId);
				return CreateToken(database.Id, database.Position.Name);
			}
			else if (await db.StaffUsers.FindAsync(check.UserId) != null)
			{
				var database = await db.StaffUsers.FindAsync(check.UserId);
				return CreateToken(database.Id, database.Position.Name);
			}
			else if (await db.Distributors.FindAsync(check.UserId) != null)
			{
				var database = await db.Distributors.FindAsync(check.UserId);
				return CreateToken(database.Id, database.Position.Name);
			}
			else
			{
				return string.Empty;
			}*/
			#endregion
		}
		if (check.Exipres < DateTime.Now)
		{
			db.ApiTokens.Remove(check);
			await db.SaveChangesAsync();
			return false;
		}
		else
		{
			return false;
		}
	}

	public async Task<string> GenerrateJwt(Guid id, string role)
	{
		return CreateToken(id, role);
	}

	public async Task<bool> RevokeToken(Guid id)
	{
		if(await db.ApiTokens.FindAsync(id) == null)
		{
			return false;
		}
		else
		{
			db.ApiTokens.Remove(await db.ApiTokens.FindAsync(id));
			await db.SaveChangesAsync();
			return true;
		}
	}
}
