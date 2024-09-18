using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.DTO;
using Project_2_Web_API.Models;

namespace Project_2_Web_Api.Service.Impl;

public class GrantPermissionServiceImpl : GrantPermissionService
{
	private DatabaseContext db;
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IMapper mapper;
	public GrantPermissionServiceImpl(DatabaseContext db, 
		IHttpContextAccessor httpContextAccessor,
		IMapper mapper
		)
	{
		this.db = db;
		this.mapper = mapper;
		this.httpContextAccessor = httpContextAccessor;
	}


	public async Task<IActionResult> Create(GrantPermissionDTO grantPermissionDTO)
	{
		var grantPermission = mapper.Map<GrantPermission>(grantPermissionDTO);
		var modelState = httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{

			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{

				if (await db.GrantPermissions.FirstOrDefaultAsync(x => x.Permission.ToLower() == grantPermission.Permission.ToLower()) != null)
				{
					return new BadRequestObjectResult(new { msg = "Permission name already exist!" });
				}
				db.GrantPermissions.Add(grantPermission);
				int check = await db.SaveChangesAsync();
				if (check > 0)
				{
					return new OkObjectResult(new { msg = true});
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

	public async Task<IActionResult> Update(int id, GrantPermissionDTO grantPermissionDTO)
	{
		var grantPermission = mapper.Map<GrantPermission>(grantPermissionDTO);
		var modelState = httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
				if (modelState != null && !modelState.IsValid)
				{
					return new BadRequestObjectResult(modelState);
				}
				else
				{
					if ( await db.GrantPermissions.FirstOrDefaultAsync(x => x.Permission.ToLower() == grantPermission.Permission.ToLower() && x.Id != id) != null)
					{
						return new BadRequestObjectResult(new { msg = "Permission name already exist!" });
					}
					var data = await db.GrantPermissions.FindAsync(id);
				if (data == null)
				{
					return new BadRequestObjectResult(new { msg = "Id does not exist!" });
				}
					data.Permission = grantPermission.Permission;
					data.Module = grantPermission.Module;
					db.Entry(data).State = EntityState.Modified;
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
		}catch(Exception ex)
		{
			return new BadRequestObjectResult(new { error = ex.Message });
		}
	}

	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			if (await db.GrantPermissions.FindAsync(id) == null)
			{
				return new BadRequestObjectResult(new { msg = "Id does not exist!" });
			}
			else
			{
				db.GrantPermissions.Remove(await db.GrantPermissions.FindAsync(id));
				var check = await db.SaveChangesAsync();
				if ( check > 0)
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

	public async Task<dynamic> FindAll()
	{
		try
		{
			if( await db.GrantPermissions.AnyAsync() == false)
			{
				return new {msg = "Data is null !!!"};
			}

			return await db.GrantPermissions.Select(x=>new
			{
				id = x.Id,
				module = x.Module,
				permission = x.Permission,
			}).ToListAsync();
		}
		catch(Exception ex)
		{
			return new { msg = ex.Message };
		}
	}

	public async Task<dynamic> FindById(int id)
	{
		try
		{
			if (await db.GrantPermissions.AnyAsync() == false || await db.GrantPermissions.FindAsync(id) == null)
			{
				return new { msg = "Data is null !!!" };
			}

			return await db.GrantPermissions.Where(x=>x.Id ==id).Select(x => new
			{
				id = x.Id,
				module = x.Module,
				permission = x.Permission,
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
			if (await db.GrantPermissions.AnyAsync() == false ||await db.GrantPermissions.Where(x => x.Permission.ToLower().Contains(name.ToLower())).AnyAsync() == false)  
			{
				 return new { msg = "Data is null !!!" }; ;
			}

			return await db.GrantPermissions.Where(x => x.Permission.ToLower().Contains(name.ToLower())).Select(x => new
			{
				id = x.Id,
				module = x.Module,
				permission = x.Permission,
			}).ToListAsync();
		}
		catch (Exception ex)
		{
			return new { msg = ex.Message };
		}
	}
}
