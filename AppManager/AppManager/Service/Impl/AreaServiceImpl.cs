using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.DTO;
using Project_2_Web_API.Models;

namespace Project_2_Web_Api.Service.Impl;

public class AreaServiceImpl : AreaService
{
	private DatabaseContext db;
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IMapper mapper;
	private readonly UserServiceAccessor userServiceAccessor;
	public AreaServiceImpl(DatabaseContext db,
		IHttpContextAccessor httpContextAccessor,
		IMapper mapper, UserServiceAccessor userServiceAccessor
		)
	{
		this.userServiceAccessor = userServiceAccessor;
		this.db = db;
		this.mapper = mapper;
		this.httpContextAccessor = httpContextAccessor;
	}

	public async Task<IActionResult> Create(AreaDTO areaDTO)
	{
		var area = mapper.Map<Area>(areaDTO);
		var modelState = httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if (await userServiceAccessor.CheckPermission("Create new area") || await userServiceAccessor.IsSystem())
				{
					if (await db.Areas.FirstOrDefaultAsync(x => x.Code.ToLower() == area.Code.ToLower()) != null)
					{
						return new BadRequestObjectResult(new { msg = "Code already exist!" });
					}
					db.Areas.Add(area);
					int check = await db.SaveChangesAsync();
					if (check > 0)
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

	public async Task<IActionResult> Update(int id, string nameArea)
	{ 
		try
		{
			if (nameArea == null)
			{
				return new BadRequestObjectResult(new { msg = "Name Area required !!" });
			}
			else
			{
				if (await userServiceAccessor.CheckPermission("Update area detail ") || await userServiceAccessor.IsSystem())
				{
					var area = await db.Areas.FindAsync(id);
					if (area == null)
					{
						return new BadRequestObjectResult(new { msg = "Id Area not exist !!" });
					}
					area.Name = nameArea;
					db.Entry(area).State = EntityState.Modified;
					int check = await db.SaveChangesAsync();
					if (check > 0)
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

	public async Task<IActionResult> AddStaffToArea(int idArea, string _idStaff)
	{
		Guid idStaff;
		bool parseGuid = Guid.TryParse(_idStaff, out idStaff);
		try
		{
			if (parseGuid ==false)
			{
			return new BadRequestObjectResult(new { msg = "Id Statff invalid !!" });
			}
			if (await db.StaffUsers.FindAsync(idStaff) == null)
			{
				return new BadRequestObjectResult(new { msg = "ID Staff does not exist!" });
			}
			var area = await db.Areas.FirstOrDefaultAsync(x => x.Id == idArea);
			if(area == null)
			{
				return new BadRequestObjectResult(new { msg = "ID Area does not exist!" });
			}
			var staff = await db.StaffUsers.FirstOrDefaultAsync(x=>x.Id == idStaff);
			area.StaffUsers.Add(staff);
			db.Entry(area).State = EntityState.Modified;
			int check = await db.SaveChangesAsync();
			if (check > 0)
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

	public async Task<IActionResult> RemoveStaffInArea(int idArea, string _idStaff)
	{
		Guid idStaff;
		bool parseGuid = Guid.TryParse(_idStaff, out idStaff);
		try
		{
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id Statff invalid !!" });
			}
			if (await db.StaffUsers.FindAsync(idStaff) == null)
			{
				return new BadRequestObjectResult(new { msg = "ID Staff does not exist!" });
			}
			var area = await db.Areas.FirstOrDefaultAsync(x => x.Id == idArea);
			if (area == null)
			{
				return new BadRequestObjectResult(new { msg = "ID Area does not exist!" });
			}
			
				area.StaffUsers.Remove(await db.StaffUsers.FindAsync(idStaff));
			
			db.Entry(area).State = EntityState.Modified;
			int check = await db.SaveChangesAsync();
			if (check > 0)
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

	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			if (await db.Areas.FindAsync(id) == null)
			{
				return new BadRequestObjectResult(new { msg = "Id does not exist!" });
			}
			else
			{
				if (await userServiceAccessor.CheckPermission("Delete areas") || await userServiceAccessor.IsSystem())
				{
					var data = await db.Areas.FindAsync(id);
					data.StaffUsers.Clear();
					data.Distributors.Clear();
					db.Areas.Remove(data);
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

	public async Task<dynamic> FindAll()
	{
		try
		{

			if(await db.Areas.AnyAsync()==false)
			{
				return new { msg = "Data is null !!!" };
			}
			return await db.Areas.Select(x => new
			{
				id = x.Id,
				code = x.Code,
				name = x.Name,
				staffs = x.StaffUsers.Select(i => new
				{
					id = i.Id,
					fullname = i.Fullname,
					email = i.Email,
					position = i.Position.Name,
					isstatus = i.IsStatus/*==true? "Activated" : "Not activated"*/
				}).ToList(),
				distributors = x.Distributors.Select(j => new
				{
					id=j.Id,
					name = j.Name,
					email = j.Email,
					address = j.Address,
					phone = j.PhoneNumber,
					position = j.Position.Name,
					isstatus = j.IsStatus/* == true ? "Activated" : "Not activated"*/
				})
			}).ToListAsync();
		}catch(Exception ex)
		{
			return new {msg =  ex.Message};
		}
	}

	public async Task<dynamic> FindById(int id)
	{
		try
		{

			if (await db.Areas.AnyAsync() == false || await db.Areas.FindAsync(id)==null)
			{
				return new { msg = "Data is null !!!" };
			}
			return await db.Areas.Where(a=>a.Id==id).Select(x => new
			{
				id = x.Id,
				code = x.Code,
				name = x.Name,
				staffs = x.StaffUsers.Select(i => new
				{
					id = i.Id,
					fullname = i.Fullname,
					email = i.Email,
					position = i.Position.Name,
					isstatus = i.IsStatus == true ? "Actived" : "Activated"
				}).ToList(),
				distributors = x.Distributors.Select(j => new
				{
					id = j.Id,
					name = j.Name,
					email = j.Email,
					address = j.Address,
					phone = j.PhoneNumber,
					position = j.Position.Name,
					isstatus = j.IsStatus == true ? "Actived" : "Activated"
				})
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

			if (await db.Areas.AnyAsync() == false || await db.Areas.Where(a => a.Name.ToLower().Contains(name.ToLower())).AnyAsync() == false)
			{
				return new { msg = "Data is null !!!" };
			}
			return await db.Areas.Where(a=>a.Name.ToLower().Contains(name.ToLower())).Select(x => new
			{
				id = x.Id,
				code = x.Code,
				name = x.Name,
				staffs = x.StaffUsers.Select(i => new
				{
					id = i.Id,
					fullname = i.Fullname,
					email = i.Email,
					position = i.Position.Name,
					isstatus = i.IsStatus == true ? "Actived" : "Activated"
				}).ToList(),
				distributors = x.Distributors.Select(j => new
				{
					id = j.Id,
					name = j.Name,
					email = j.Email,
					address = j.Address,
					phone = j.PhoneNumber,
					position = j.Position.Name,
					isstatus = j.IsStatus == true ? "Actived" : "Activated"
				})
			}).ToListAsync();
		}
		catch (Exception ex)
		{
			return new { msg = ex.Message };
		}
	}

	public async Task<IActionResult> AddDistributorToArea(int idArea, string _idDistributor)
	{
		Guid idDistributor;
		bool parseGuid = Guid.TryParse(_idDistributor, out idDistributor);
		try
		{
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id Distributor invalid !!" });
			}
			if (await db.Distributors.FindAsync(idDistributor) == null)
			{
				return new BadRequestObjectResult(new { msg = "ID Distributor does not exist!" });
			}
			var area = await db.Areas.FirstOrDefaultAsync(x => x.Id == idArea);
			if (area == null)
			{
				return new BadRequestObjectResult(new { msg = "ID Area does not exist!" });
			}
			var distributor = await db.Distributors.FirstOrDefaultAsync(x => x.Id == idDistributor);
			area.Distributors.Add(distributor);
			db.Entry(area).State = EntityState.Modified;
			int check = await db.SaveChangesAsync();
			if (check > 0)
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

	public async Task<IActionResult> RemoveDistributorInArea(int idArea, string _idDistributor)
	{
		Guid idDistributor;
		bool parseGuid = Guid.TryParse(_idDistributor, out idDistributor);
		try
		{
			if (parseGuid == false)
			{
				return new BadRequestObjectResult(new { msg = "Id Distributor invalid !!" });
			}
			if (await db.Distributors.FindAsync(idDistributor) == null)
			{
				return new BadRequestObjectResult(new { msg = "ID Distributor does not exist!" });
			}
			var area = await db.Areas.FirstOrDefaultAsync(x => x.Id == idArea);
			if (area == null)
			{
				return new BadRequestObjectResult(new { msg = "ID Area does not exist!" });
			}
			
				area.Distributors.Remove( await db.Distributors.FindAsync(idDistributor));
			
			db.Entry(area).State = EntityState.Modified;
			int check = await db.SaveChangesAsync();
			if (check > 0)
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

	public async Task<IActionResult> DetailStaffInAre(int id)
	{
		try
		{

			if (await db.Areas.AnyAsync() == false)
			{
				return new BadRequestObjectResult( new { msg = "Data is null !!!" });
			}
			return new OkObjectResult( await db.Areas.Where(x=>x.Id==id).Select(x => new
			{
				id = x.Id,
				code = x.Code,
				name = x.Name,
				staffs = x.StaffUsers.Select(i => new
				{
					id = i.Id,
					fullname = i.Fullname,
					email = i.Email,
					positionId = i.PositionId,
					position = i.Position.Name,
					isstatus = i.IsStatus/*==true? "Activated" : "Not activated"*/
				}).OrderBy(j=>j.positionId).ToList(),
			}).FirstOrDefaultAsync());
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult( new { msg = ex.Message });
		}
	}
}
