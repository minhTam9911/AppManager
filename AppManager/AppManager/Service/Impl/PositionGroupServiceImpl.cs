using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project_2_Web_Api.DTO;
using Project_2_Web_API.Models;

namespace Project_2_Web_Api.Service.Impl;

public class PositionGroupServiceImpl : PositionGroupService
{
	private DatabaseContext db;
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IMapper mapper;
	public PositionGroupServiceImpl(DatabaseContext db, IHttpContextAccessor httpContextAccessor, IMapper mapper)
	{
		this.db = db;
		this.httpContextAccessor = httpContextAccessor;
		this.mapper = mapper;
	}

	public async Task<IActionResult> Delete(string id)
	{
		try
		{
			if(await db.PositionGroups.FindAsync(Int32.Parse(id)) == null)
			{
				return new BadRequestObjectResult(new { msg = "id does not exist!" });
			}
			else
			{
				db.PositionGroups.Remove(await db.PositionGroups.FindAsync(Int32.Parse(id)));
				var check = db.SaveChangesAsync();
				if (await check > 0)
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
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<dynamic> FindAll()
	{
		try
		{
			if ( await db.PositionGroups.AnyAsync() == false)
			{
				return new { msg = "Data is null !!!" };
			}
			else
			{
				return await db.PositionGroups.Select(x=>new {
					id = x.Id, name = x.Name,created = x.Created
				}).ToListAsync();
			}
		}
		catch(Exception ex)
		{
			return new { msg = ex.Message };
		}
	}

	public async Task<dynamic> FindById(string id)
	{

		try
		{
			if (await db.PositionGroups.AnyAsync() == false || await db.PositionGroups.FindAsync(Int32.Parse(id)) == null)
			{
				return new { msg = "Data is null !!!" };
			}
			else
			{
				return await db.PositionGroups.Where(i=>i.Id == Int32.Parse(id)).Select(x => new {
					id = x.Id,
					name = x.Name,
					created = x.Created
				}).FirstOrDefaultAsync();
			}
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
			if (await db.PositionGroups.AnyAsync() == false 
				|| await db.PositionGroups.Where(i => i.Name.ToLower().Contains(name.ToLower())).AnyAsync() == false)
			{
				return new { msg = "Data is null !!!" };
			}
			else
			{
				return await db.PositionGroups.Where(i=>i.Name.ToLower().Contains(name.ToLower())).Select(x => new {
					id = x.Id,
					name = x.Name,
					created = x.Created
				}).ToListAsync();
			}
		}
		catch (Exception ex)
		{
			return new { msg = ex.Message };
		}
	}

	public async Task<IActionResult> Create(PositionGroupDTO positionGroupDTO)
	{
		var positionGroup = mapper.Map<PositionGroup>(positionGroupDTO);
		var modelState = httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
			
			
				if (modelState != null && !modelState.IsValid)
				{
					return new BadRequestObjectResult(modelState);
				}
				else
				{
					
						if (await db.PositionGroups.FirstOrDefaultAsync(x => x.Name.ToLower() == positionGroup.Name.ToLower()) != null)
						{
							return new BadRequestObjectResult(new { msg = "Name Position Group already exist!" });
						}
						positionGroup.Created = DateTime.Now;
						db.PositionGroups.Add(positionGroup);
						int check = await db.SaveChangesAsync();
						if (check > 0)
						{
							return new OkObjectResult(new { msg = true });
						}
						else
						{
							return new BadRequestObjectResult(new { msg =false });
						}

					
				}
			

		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> Update(string id, PositionGroupDTO positionGroupDTO)
	{
		var positionGroup = mapper.Map<PositionGroup>(positionGroupDTO);
		var modelState = httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
			

				if (modelState != null && !modelState.IsValid)
				{
					return new BadRequestObjectResult(modelState);
				}
				else
				{

					if (await db.PositionGroups.FirstOrDefaultAsync(x => x.Name.ToLower() == positionGroup.Name.ToLower() && x.Id !=int.Parse(id)) != null)
					{
						return new BadRequestObjectResult(new { msg = "Name Position Group already exist!" });
					}
					var data = await db.PositionGroups.FindAsync(Int32.Parse(id));
					data.Name = positionGroup.Name;
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
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}
}