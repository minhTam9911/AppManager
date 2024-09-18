using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.DTO;
using Project_2_Web_API.Models;

namespace Project_2_Web_Api.Service.Impl;

public class PositionServiceImpl : PositionService
{
	private DatabaseContext db;
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IMapper mapper;

	public PositionServiceImpl(DatabaseContext db, IHttpContextAccessor httpContextAccessor, IMapper mapper)
	{
		this.mapper = mapper;
		this.db = db;
		this.httpContextAccessor = httpContextAccessor;
	}

	public async Task<IActionResult> Create(PositionDTO positionDTO)
	{
		var position = mapper.Map<Position>(positionDTO);
		var modelState = httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
				if (modelState != null && !modelState.IsValid)
				{
					return new BadRequestObjectResult(modelState);
				}
				else
				{
					if (await db.PositionGroups.FindAsync(position.PositionGroupId) == null)
					{
						return new BadRequestObjectResult(new { msg = "Position Group not exist!" });
					}

					if (await db.Positions.FirstOrDefaultAsync(x => x.Name.ToLower() == position.Name.ToLower()) != null)
					{
						return new BadRequestObjectResult(new { msg = "Name Position already exist!" });
					}
					position.Created = DateTime.Now;
					db.Positions.Add(position);
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
		catch(Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> Update(int id, PositionDTO positionDTO)
	{
		var position = mapper.Map<Position>(positionDTO);
		var modelState = httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
			

				if (modelState != null && !modelState.IsValid)
				{
					return new BadRequestObjectResult(modelState);
				}
				else
				{
					if(await db.Positions.FindAsync(id) == null)
					{
						return new BadRequestObjectResult(new { msg = "Id does not exist! " });
					}
					
					if (await db.Positions.Where(x => x.Name == position.Name && x.Id != id).AnyAsync())
					{
							return new BadRequestObjectResult(new { msg = "Name Position already exist!" });
					}
					if(await db.PositionGroups.FindAsync(position.PositionGroupId) == null)
				{
					return new BadRequestObjectResult(new { msg = "Position Group does not exist!" });
				}
					var data = await db.Positions.FindAsync(id);
					data.Name = position.Name;
					data.PositionGroupId = position.PositionGroupId;
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

	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			if (await db.Positions.FindAsync(id) == null)
			{
				return new BadRequestObjectResult(new { msg = "Id does not exist!" });
			}
			else
			{
				db.Positions.Remove(await db.Positions.FindAsync(id));
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
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { error = ex.Message });
		}
	}

	public async Task<dynamic> FindAll()
	{

		try
		{
			if (await db.Positions.AnyAsync() == false)
			{
				return new { msg = "Data is null !!!" };
			}
			else
			{
				return await db.Positions.Select( x => new {
					id = x.Id,
					name = x.Name,
					positionGroupId = x.PositionGroupId,
					positionGroupName = x.PositionGroup.Name,
					created = x.Created
				}).OrderBy(x=>x.positionGroupId).ToListAsync();
			}
		}
		catch (Exception ex)
		{
			return new { msg = ex.Message };
		}
	}

	public async Task<dynamic> FindById(int id)
	{
		try
		{
			if (await db.Positions.AnyAsync() == false || await db.Positions.FindAsync(id) == null)
			{
				return new { msg = "Data is null !!!" };
			}
			else
			{
				return await db.Positions.Where(i=>i.Id == id).Select(x => new {
					id = x.Id,
					name = x.Name,
					positionGroupId = x.PositionGroupId,
					positionGroupName = x.PositionGroup.Name,
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
			if (await db.Positions.AnyAsync() == false || await db.Positions.Where(x => x.Name.ToLower().Contains(name.ToLower())).AnyAsync() == false)
			{
				return new { msg = "Data is null !!!" };
			}
			else
			{
				return await db.Positions.Where(x => x.Name.ToLower().Contains(name.ToLower())).Select(x => new
				{
					id = x.Id,
					name = x.Name,
					positionGroupId = x.PositionGroupId,
					positionGroupName = x.PositionGroup.Name,
					created = x.Created
				}).ToListAsync();
			}
		}
		catch (Exception ex)
		{
			return new { msg = ex.Message };
		}
	}
}
