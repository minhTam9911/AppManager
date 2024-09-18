using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Helplers;
using Project_2_Web_API.Models;
using System.Security.Claims;

namespace Project_2_Web_Api.Service.Impl;

public class DistributorServiceImpl : DistributorService
{

	private DatabaseContext db;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IMapper mapper;
	private IConfiguration configuration;
	private readonly UserServiceAccessor userServiceAccessor;
	public DistributorServiceImpl(DatabaseContext db,UserServiceAccessor userServiceAccessor, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper)
	{
		this.mapper = mapper;
		this.userServiceAccessor = userServiceAccessor;
		this.configuration = configuration;
		_httpContextAccessor = httpContextAccessor;
		this.db = db;
	}

	public async Task<IActionResult> Create(DistributorDTO distributorDTO)
	{
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		var distributor = mapper.Map<Distributor>(distributorDTO);
		try
		{

			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			if(distributor.Address == null)
			{
				modelState.AddModelError("Address", "The Address is requied!!");
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if (await userServiceAccessor.CheckPermission("Create new distributor") || await userServiceAccessor.IsSystem())
				{
					if (await db.Users.FirstOrDefaultAsync(x => x.Email == distributor.Email) != null)
					{
						return new BadRequestObjectResult(new { error = "Email already exist!!" });
					}
					if (await db.StaffUsers.FirstOrDefaultAsync(x => x.Email == distributor.Email) != null)
					{
						return new BadRequestObjectResult(new { error = "Email already exist!!" });
					}
					if (await db.Distributors.FirstOrDefaultAsync(x => x.Email == distributor.Email) != null)
					{
						return new BadRequestObjectResult(new { error = "Email already exist!!" });
					}
					if (await db.Positions.FindAsync(distributor.PositionId) == null)
					{
						return new BadRequestObjectResult(new { error = "Position not exist!!" });
					}
					var checkPosition = await db.Positions.FindAsync(distributor.PositionId);
					if (checkPosition.Name.ToLower() != "distributor - om/tl")
					{
						return new BadRequestObjectResult(new { msg = "Position option is invalid. You can only choose 'Distributor - OM/TL'" });
					}
					distributor.CreateBy = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name));
					distributor.CreatedDate = DateTime.Now;
					distributor.PhotoAvatar = "avatar-default-icon.png";
					var password = RandomHelper.RandomDefaultPassword(12);
					var mailHelper = new MailHelper(configuration);
					Guid idSaleManagement;
					bool parseGuid1 = Guid.TryParse(distributorDTO.IdSaleManagement, out idSaleManagement);

					if (parseGuid1 == false)
					{
						return new BadRequestObjectResult(new { msg = "Id Sale Management invalid !!" });
					}
					if (await db.StaffUsers.FindAsync(idSaleManagement) == null)
					{
						return new BadRequestObjectResult(new { msg = "Id Sale Management not exist !!" });
					}
					if (!distributorDTO.IdSales.IsNullOrEmpty())
					{
						Guid idSales;
						bool parseGuid2 = Guid.TryParse(distributorDTO.IdSales, out idSales);
						if (parseGuid1 == false)
						{
							return new BadRequestObjectResult(new { msg = "Id Sales invalid !!" });
						}
						if (await db.StaffUsers.FindAsync(idSales) == null)
						{
							return new BadRequestObjectResult(new { msg = "Id Sales not exist !!" });
						}
						distributor.SalesId = idSales;
					}
					distributor.SaleManagementId = idSaleManagement;
					var check = mailHelper.Send(configuration["Gmail:Username"], distributor.Email, "Welcome " + distributor.Name + " to join CDExcellent", MailHelper.HtmlNewAccount(distributor.Name, distributor.Email, password));
					if (!check)
					{
						return new BadRequestObjectResult(new { msg = "Email sending failed." });
					}
					var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
					distributor.Password = hashPassword;
					db.Distributors.Add(distributor);
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
					return new UnauthorizedResult();
				}

			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}

	}

	public async Task<IActionResult> Update(string id, DistributorDTO distributorDTO)
	{
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		var distributor = mapper.Map<Distributor>(distributorDTO);
		Guid idDistributor;
		bool parseGuid = Guid.TryParse(id, out idDistributor);
		try
		{
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id Distributor invalid !!" });
			}
			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if (await userServiceAccessor.CheckPermission("Update detail distributor") || await userServiceAccessor.IsSystem())
				{
					if (await db.Users.FirstOrDefaultAsync(x => x.Email == distributor.Email) != null)
					{
						return new BadRequestObjectResult(new { error = "Email already exist!!" });
					}
					if (await db.StaffUsers.FirstOrDefaultAsync(x => x.Email == distributor.Email) != null)
					{
						return new BadRequestObjectResult(new { error = "Email already exist!!" });
					}
					if (await db.Distributors.FirstOrDefaultAsync(x => x.Email == distributor.Email && x.Id != idDistributor) != null)
					{
						return new BadRequestObjectResult(new { error = "Email already exist!!" });
					}
					if (await db.Positions.FindAsync(distributor.PositionId) == null)
					{
						return new BadRequestObjectResult(new { error = "Position not exist!!" });
					}
					var checkPosition = await db.Positions.FindAsync(distributor.PositionId);
					if (checkPosition.Name.ToLower() != "distributor - om/tl")
					{
						return new BadRequestObjectResult(new { msg = "Position option is invalid. You can only choose 'Distributor - OM/TL'" });
					}
					else
					{
						var data = await db.Distributors.FindAsync(idDistributor);
						if (data == null)
						{
							return new BadRequestObjectResult(new { msg = "Id Distributor not exist!!" });
						}
						Guid idSaleManagement;
						bool parseGuid1 = Guid.TryParse(distributorDTO.IdSaleManagement, out idSaleManagement);

						if (parseGuid1 == false)
						{
							return new BadRequestObjectResult(new { msg = "Id Sale Management invalid !!" });
						}
						if (await db.StaffUsers.FindAsync(idSaleManagement) == null)
						{
							return new BadRequestObjectResult(new { msg = "Id Sale Management not exist !!" });
						}
						if (distributorDTO.IdSales != null)
						{
							Guid idSales;
							bool parseGuid2 = Guid.TryParse(distributorDTO.IdSales, out idSales);
							if (parseGuid1 == false)
							{
								return new BadRequestObjectResult(new { error = "Id Sales invalid !!" });
							}
							if (await db.StaffUsers.FindAsync(idSales) == null)
							{
								return new BadRequestObjectResult(new { msg = "Id Sales not exist !!" });
							}
							distributor.SalesId = idSales;
							data.SalesId = distributor.SalesId;
						}
						distributor.SaleManagementId = idSaleManagement;
						data.IsStatus = distributor.IsStatus;
						data.Name = distributor.Name;
						data.Email = distributor.Email;
						data.PositionId = distributor.PositionId;
						data.SaleManagementId = distributor.SaleManagementId;
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
				else
				{
					return new UnauthorizedResult();
				}
			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { error = ex.Message });
		}
	}

	public async Task<IActionResult> Delete(string id)
	{
		Guid idDistributor;
		bool parseGuid = Guid.TryParse(id, out idDistributor);
		try
		{
			if (await userServiceAccessor.CheckPermission("Delete distributor") || await userServiceAccessor.IsSystem())
			{
				if (parseGuid == false)
				{
					return new BadRequestObjectResult(new { msg = "Id Distributor invalid !!" });
				}
				if (await db.Distributors.FindAsync(idDistributor) == null)
				{
					return new BadRequestObjectResult(new { msg = "Id Distributor does not exist!" });
				}
				else
				{
					db.Distributors.Remove(await db.Distributors.FindAsync(idDistributor));
					var check = await db.SaveChangesAsync();
					if (check > 0)
					{
						return new OkObjectResult(new { msg = true });
					}
					else
					{
						return new BadRequestObjectResult(new { msg = false });
					}
				}
			}
			else
			{
				return new UnauthorizedResult();
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

			if (await db.Distributors.AnyAsync() == false)
			{
				return new { msg = "Data is null !!!" };
			}
			return await db.Distributors.Select(x => new
			{
				id = x.Id,
				name = x.Name,
				email = x.Email,
				address = x.Address,
				phoneNumber = x.PhoneNumber,
				saleManagementId = x.SaleManagementId,
				saleManagementName = x.SaleManagement.Fullname,
				salesId = x.SalesId,
				salesName = x.Name,
				positionId = x.PositionId,
				positionName = x.Position.Name,
				area = x.Area == null? null : new
				{
					id= x.Area.Id,
					name = x.Area.Name
				},
				status = x.IsStatus,
				createBy = x.StaffUser.Fullname

			}).ToListAsync();
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<dynamic> FindById(string id)
	{
		Guid idDistributor;
		bool parseGuid = Guid.TryParse(id, out idDistributor);
		try
		{
			if (parseGuid == false)
			{
				return new { error = "Id Distributor invalid !!" };
			}
			if (await db.Distributors.AnyAsync() == false || await db.Distributors.FindAsync(idDistributor) == null)
			{
				return new { error = "Data is null !!!" };
			}
			return await db.Distributors.Where(x => x.Id == idDistributor).Select(x => new
			{
				id = x.Id,
				name = x.Name,
				email = x.Email,
				address = x.Address,
				phoneNumber = x.PhoneNumber,
				saleManagementId = x.SaleManagementId,
				saleManagementName = x.SaleManagement.Fullname,
				salesId = x.SalesId,
				salesName = x.Name,
				positionId = x.PositionId,
				positionName = x.Position.Name,
				area = x.Area == null ? null : new
				{
					id = x.Area.Id,
					name = x.Area.Name
				},
				status = x.IsStatus,
				createBy = x.StaffUser.Fullname

			}).FirstOrDefaultAsync();
		}
		catch (Exception ex)
		{
			return new { msg = ex.Message };
		}
	}

	public async Task<dynamic> FindByName(string name)
	{
		try
		{

			if (await db.Distributors.AnyAsync() == false
				|| await db.Distributors.Where(x => x.Name.ToLower().Contains(name.ToLower())).AnyAsync() == false)
			{
				return new { msg = "Data is null !!!" };
			}
			return await db.Distributors.Where(x => x.Name.ToLower().Contains(name.ToLower())).Select(x => new
			{
				id = x.Id,
				name = x.Name,
				email = x.Email,
				address = x.Address,
				phoneNumber = x.PhoneNumber,
				saleManagementId = x.SaleManagementId,
				saleManagementName = x.SaleManagement.Fullname,
				salesId = x.SalesId,
				salesName = x.Name,
				positionId = x.PositionId,
				positionName = x.Position.Name,
				area = x.Area == null ? null : new
				{
					id = x.Area.Id,
					name = x.Area.Name
				},
				status = x.IsStatus,
				createBy = x.StaffUser.Fullname

			}).ToListAsync();
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> SettingPermission(string id, int[] permissions)
	{
		Guid idDistributor;
		bool parseGuid = Guid.TryParse(id, out idDistributor);
		try
		{
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id Distributor invalid !!" });
			}
			var data = await db.Distributors.FindAsync(idDistributor);
			if (data == null)
			{
				return new BadRequestObjectResult(new { msg = "Id Distributor does not exist !!" });
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
			return new OkObjectResult(new { msg = "Add permissions to user successfully" });
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
			Guid idDistributor;
			bool parseGuid = Guid.TryParse(id, out idDistributor);
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id Distributor invalid !!" });
			}
			var distributor = await db.Distributors.FindAsync(idDistributor);
			if (distributor == null)
			{
				return new BadRequestObjectResult(new { msg = "Id Distributor does not exist !!" });
			}
			var password = RandomHelper.RandomDefaultPassword(12);
			var mailHelper = new MailHelper(configuration);
			var content = "<h2>CDExcellent</h2><br><br>" +
						"<h2>Hello " + distributor.Name + "!</h2><br>" +
					"<h3>CDExcellent is glad you signed up.</h3><br>" +
					"<h2>=>Your account: " + distributor.Email + "</h2><br>" +
					"<h2>=>Password: " + password + "</h2><br>" +
					"<h3>This is only a temporary password, please log in and change it.</h3><br>" +
					"<h2>Thank you very much!</h2>";
			var check = mailHelper.Send(configuration["Gmail:Username"], distributor.Email, "Reset password account CDExcellent", content);
			if (!check)
			{
				return new BadRequestObjectResult(new { msg = "Email sending failed." });
			}
			var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
			distributor.Password = hashPassword;
			db.Entry(distributor).State = EntityState.Modified;
			if (await db.SaveChangesAsync() > 0)
			{
				return new OkObjectResult(new { msg = true });
			}
			else
			{
				return new BadRequestObjectResult(new { msg = false });
			}

		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}
}
