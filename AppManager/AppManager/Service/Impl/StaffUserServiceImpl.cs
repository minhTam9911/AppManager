

using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Helplers;
using Project_2_Web_API.Models;
using System.Security.Claims;

namespace Project_2_Web_Api.Service.Impl;

public class StaffUserServiceImpl : StaffUserService
{
	private DatabaseContext db;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IMapper mapper;
	private IConfiguration configuration;
	public StaffUserServiceImpl(DatabaseContext db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper)
	{
		this.mapper = mapper;
		this.configuration = configuration;
		_httpContextAccessor = httpContextAccessor;
		this.db = db;
	}

	public async Task<IActionResult> Create(StaffUserDTO staffUserDTO)
	{
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		var staffUser = mapper.Map<StaffUser>(staffUserDTO);
		try
		{

			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if (await db.Users.FirstOrDefaultAsync(x => x.Email == staffUser.Email) != null)
				{
					return new BadRequestObjectResult(new { msg = "Email already exist!!" });
				}
				if (await db.StaffUsers.FirstOrDefaultAsync(x => x.Email == staffUser.Email) != null)
				{
					return new BadRequestObjectResult(new { msg = "Email already exist!!" });
				}
				if (await db.Distributors.FirstOrDefaultAsync(x => x.Email == staffUser.Email) != null)
				{
					return new BadRequestObjectResult(new { msg = "Email already exist!!" });
				}
				if (await db.Positions.FindAsync(staffUser.PositionId) == null)
				{
					return new BadRequestObjectResult(new { msg = "Position not exist!!" });
				}
				var checkPosition = await db.Positions.FindAsync(staffUser.PositionId);
				if (checkPosition.Name.ToLower() == "guest" || checkPosition.Name == "other department")
				{
					return new BadRequestObjectResult(new { msg = "Position option is invalid. Do not select Guest or Other Department" });
				}
				staffUser.CreateBy = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
				staffUser.CreatedDate = DateTime.Now;
				var password = RandomHelper.RandomDefaultPassword(12);
				var mailHelper = new MailHelper(configuration);
				var check = mailHelper.Send(configuration["Gmail:Username"], staffUser.Email, "Welcome " + staffUser.Fullname + " to join CDExcellent", MailHelper.HtmlNewAccount(staffUser.Fullname,staffUser.Email,password));
				if (!check)
				{
					return new BadRequestObjectResult(new { msg = "Email sending failed." });
				}
				var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
				staffUser.Password = hashPassword;
				if(staffUser.PositionId != 1 && staffUser.PositionId != 2)
				{
					foreach (var staffSuperior in staffUserDTO.StaffSuperiorId)
					{
						Guid idStaffSuperior;
						bool parseGuid = Guid.TryParse(staffSuperior, out idStaffSuperior);
						if (parseGuid == false)
						{
							return new BadRequestObjectResult(new { msg = "Id Staff Superior invalid !!" });
						}
						var dataSuperior = await db.StaffUsers.FindAsync(idStaffSuperior);
						if (dataSuperior == null)
						{
							return new BadRequestObjectResult(new { msg = "Id Staff Superior not exist !!" });
						}
						staffUser.StaffSuperior.Add(dataSuperior);
					}

					foreach (var staffInterior in staffUserDTO.StaffInteriorId)
					{
						if (!staffInterior.IsNullOrEmpty())
						{
							Guid idStaffInterior;
							bool parseGuid = Guid.TryParse(staffInterior, out idStaffInterior);
							if (parseGuid == false)
							{
								return new BadRequestObjectResult(new { msg = "Id Staff Interior invalid !!" });
							}
							var dataInterior = await db.StaffUsers.FindAsync(idStaffInterior);
							if (dataInterior == null)
							{
								return new BadRequestObjectResult(new { msg = "Id Staff Interior not exist !!" });
							}
							staffUser.StaffInterior.Add(dataInterior);
						}

					}

				}

				staffUser.PhotoAvatar = "avatar-default-icon.png";
				db.StaffUsers.Add(staffUser);
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

	public async Task<IActionResult> Update(string id, StaffUserDTO staffUserDTO)
	{
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		var staffUser = mapper.Map<StaffUser>(staffUserDTO);
		Guid idStaffUser;
		bool parseGuid = Guid.TryParse(id, out idStaffUser);
		try
		{
			if (parseGuid == false)
			{
			return new BadRequestObjectResult(new { msg = "Id Staff invalid !!" });
			}
			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if (await db.Users.FirstOrDefaultAsync(x => x.Email == staffUser.Email) != null)
				{
					return new BadRequestObjectResult(new { msg = "Email already exist!1!" });
				}
				if (await db.StaffUsers.FirstOrDefaultAsync(x => x.Email == staffUser.Email && x.Id != idStaffUser) != null)
				{
					return new BadRequestObjectResult(new { msg = "Email already exist!2!" });
				}
				if (await db.Distributors.FirstOrDefaultAsync(x => x.Email == staffUser.Email) != null)
				{
					return new BadRequestObjectResult(new { msg = "Email already exist!3!" });
				}
				if (await db.Positions.FindAsync(staffUser.PositionId) == null)
				{
					return new BadRequestObjectResult(new { msg = "Position not exist!!" });
				}
				if(await db.StaffUsers.FindAsync(idStaffUser) == null)
				{
					return new BadRequestObjectResult(new { msg = "Id Staff not exist!!" });
				}
				var checkPosition = await db.Positions.FindAsync(staffUser.PositionId);
				if (checkPosition.Name.ToLower() == "guest" || checkPosition.Name == "other department")
				{
					return new BadRequestObjectResult(new { error = "Position option is invalid. Do not select Guest or Other Department" });
				}
				var data = await db.StaffUsers.FindAsync(idStaffUser);
				data.Fullname = staffUser.Fullname;
				data.Email = staffUser.Email;
				data.PositionId = staffUser.PositionId;
				data.StaffSuperior.Clear();
				foreach (var staffSuperior in staffUserDTO.StaffSuperiorId)
				{
					Guid idStaffSuperior;
					bool parseGuidStaff = Guid.TryParse(staffSuperior, out idStaffSuperior);
					if (parseGuidStaff == false)
					{
						return new BadRequestObjectResult(new { msg = "Id Staff Superior invalid !!" });
					}
					var dataSuperior = await db.StaffUsers.FindAsync(idStaffSuperior);
					if (dataSuperior == null)
					{
						return new BadRequestObjectResult(new { msg = "Id Staff Superior not exist !!" });
					}
					data.StaffSuperior.Add(dataSuperior);
				}
				data.StaffInterior.Clear();
				
				foreach (var staffInterior in staffUserDTO.StaffInteriorId)
				{
					if (!staffInterior.IsNullOrEmpty())
					{
						Guid idStaffInterior;
						bool parseGuidStaff = Guid.TryParse(staffInterior, out idStaffInterior);
						if (parseGuidStaff == false)
						{
							return new BadRequestObjectResult(new { msg = "Id Staff Interior invalid !!" });
						}
						var dataInterior = await db.StaffUsers.FindAsync(idStaffInterior);
						if (dataInterior == null)
						{
							return new BadRequestObjectResult(new { msg = "Id Staff Interior not exist !!" });
						}
						data.StaffInterior.Add(dataInterior);
					}
				}
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

	public async Task<IActionResult> Delete(string id)
	{
		Guid idStaff;
		bool parseGuid = Guid.TryParse(id, out idStaff);
		try
		{
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id User invalid !!" });
			}
			var data = await db.StaffUsers.FindAsync(idStaff);
			if (data  == null)
			{
				return new BadRequestObjectResult(new { msg = "Id does not exist!" });
			}
			if (data.Position.Name.ToLower() == "administrator" || data.Position.Name.ToLower() == "owner")
			{
				return new BadRequestObjectResult(new { msg = "This is a high-level account and cannot be deleted" });
			}
			else
			{
				data.StaffInterior.Clear();
				data.StaffSuperior.Clear();
				data.Area = null;
				db.Entry(data).State = EntityState.Modified;
				await db.SaveChangesAsync();
				db.StaffUsers.Remove(data);
				var check = await db.SaveChangesAsync();
				if (check > 0)
				{
					return new OkObjectResult(new { msg = "Delete Successfully!" });
				}
				else
				{
					return new BadRequestObjectResult(new { msg = "Delete Failed!" });
				}
			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<dynamic> FindAll()
	{
		try
		{

			if (await db.StaffUsers.AnyAsync() == false)
			{
				return new { msg = "Data is null !!!" };
			}
			return await db.StaffUsers.Select(x => new
			{
				id = x.Id,
				fullname = x.Fullname,
				email = x.Email,
				positionId = x.PositionId,
				positionName = x.Position.Name,
				createBy = x.CreateBy,
				area = x.Area == null ? null : new
				{
					id =  x.Area.Id,
					code= x.Area.Code,
					name = x.Area.Name
				},
				status = x.IsStatus,
				createDate = x.CreatedDate,
				phoneNumber = x.PhoneNumber,
				address = x.Address,
				staffSuperior = x.StaffSuperior.Select(i => new
				{
					idStaffSuperior = i.Id,
					fullname = i.Fullname,
					positionId= i.PositionId,
					positionName= i.Position.Name

				}),
				staffInterior = x.StaffInterior.Any() == false? null :  (x.StaffInterior.Select(j => new
				{
					idStaffInterior = j.Id,
					fullname = j.Fullname,
					positionId = j.PositionId,
					positionName = j.Position.Name

				}))

			}).ToListAsync();
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<dynamic> FindById(string id)
	{
		Guid idStaff;
		bool parseGuid = Guid.TryParse(id, out idStaff);
		try
		{
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id User invalid !!" });
			}
			if (await db.StaffUsers.AnyAsync() == false || await db.StaffUsers.FindAsync(idStaff) == null)
			{
				return new { msg = "Data is null !!!" };
			}
			return await db.StaffUsers.Where(x => x.Id == idStaff).Select(x => new
			{
				id = x.Id,
				fullname = x.Fullname,
				email = x.Email,
				positionId = x.PositionId,
				positionName = x.Position.Name,
				createBy = x.CreateBy,
				area = x.Area == null ? null : new
				{
					id = x.Area.Id,
					code = x.Area.Code,
					name = x.Area.Name
				},
				status = x.IsStatus,
				createDate = x.CreatedDate,
				phoneNumber = x.PhoneNumber,
				address = x.Address,
				staffSuperior = x.StaffSuperior.Select(i => new
				{
					idStaffSuperior = i.Id,
					fullname = i.Fullname,
					positionId = i.PositionId,
					positionName = i.Position.Name

				}),
				staffInterior = x.StaffInterior.Any() == false ? null : (x.StaffInterior.Select(j => new
				{
					idStaffInterior = j.Id,
					fullname = j.Fullname,
					positionId = j.PositionId,
					positionName = j.Position.Name

				}))

			}).FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<dynamic> FindByName(string name)
	{
		try
		{

			if (await db.StaffUsers.AnyAsync() == false
				|| await db.StaffUsers.Where(x => x.Fullname.ToLower().Contains(name.ToLower())).AnyAsync() == false)
			{
				return new { msg = "Data is null !!!" };
			}
			return await db.StaffUsers.Where(x => x.Fullname.ToLower().Contains(name.ToLower())).Select(x => new
			{
				id = x.Id,
				fullname = x.Fullname,
				email = x.Email,
				positionId = x.PositionId,
				positionName = x.Position.Name,
				area = x.Area == null ? null : new
				{
					id = x.Area.Id,
					code = x.Area.Code,
					name = x.Area.Name
				},
				status = x.IsStatus,
				createDate = x.CreatedDate,
				phoneNumber = x.PhoneNumber,
				address = x.Address,
				staffSuperior = x.StaffSuperior.Select(i => new
				{
					idStaffSuperior = i.Id,
					fullname = i.Fullname,
					positionId = i.PositionId,
					positionName = i.Position.Name

				}),
				staffInterior = x.StaffInterior.Any() == false ? null : (x.StaffInterior.Select(j => new
				{
					idStaffInterior = j.Id,
					fullname = j.Fullname,
					positionId = j.PositionId,
					positionName = j.Position.Name

				}))

			}).ToListAsync();
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> ResetPassword(string id)
	{

		try
		{
			Guid idStaff;
			bool parseGuid = Guid.TryParse(id, out idStaff);
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id Staff invalid !!" });
			}
			var staffUser = await db.StaffUsers.FindAsync(idStaff);
			if(staffUser == null)
			{
				return new BadRequestObjectResult(new { msg = "Id Staff does not exist !!" });
			}
			var password = RandomHelper.RandomDefaultPassword(12);
			var mailHelper = new MailHelper(configuration);
			var content = "<h2>CDExcellent</h2><br><br>" +
						"<h2>Hello " + staffUser.Fullname + "!</h2><br>" +
					"<h3>CDExcellent is glad you signed up.</h3><br>" +
					"<h2>=>Your account: " + staffUser.Email + "</h2><br>" +
					"<h2>=>Password: " + password + "</h2><br>" +
					"<h3>This is only a temporary password, please log in and change it.</h3><br>" +
					"<h2>Thank you very much!</h2>";
			var check = mailHelper.Send(configuration["Gmail:Username"], staffUser.Email, "Reset password account CDExcellent", content);
			if (!check)
			{
				return new BadRequestObjectResult(new { msg = "Email sending failed." });
			}
			var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
			staffUser.Password = hashPassword;
			db.Entry(staffUser).State = EntityState.Modified;
			if (await db.SaveChangesAsync() > 0)
			{
				return new OkObjectResult(new { msg = true });
			}
			else
			{
				return new BadRequestObjectResult(new { msg = true });
			}

		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> SettingPermission(string id, int[] permissions)
	{
		Guid idUser;
		bool parseGuid = Guid.TryParse(id, out idUser);
		try
		{
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id Staff invalid !!" });
			}
			var data = await db.StaffUsers.FindAsync(idUser);
			if (data == null)
			{
				return new BadRequestObjectResult(new { msg = "Id Staff does not exist !!" });
			}
			var permissionDB = await db.GrantPermissions.ToListAsync();
			foreach (var permision in permissions)
			{
				if (permissionDB.Any(x => x.Id == permision))
				{
					data.GrantPermissions.Add(await db.GrantPermissions.FindAsync(permision));
					db.Entry(data).State = EntityState.Modified;
					if (await db.SaveChangesAsync() > 0) ;
					else
					{
						return new BadRequestObjectResult(new { msg = "The system encountered a problem !!" });
					}
				}
				else
				{
					return new BadRequestObjectResult(new { msg = "ID Permission does not exist !!" });
				}
			}
			return new OkObjectResult(new { msg = true });
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

}
