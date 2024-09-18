using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.DTO;
using Project_2_Web_API.Models;
using System.Security.Claims;
using System.Web.Http.ModelBinding;

namespace Project_2_Web_Api.Service.Impl;

public class CommentServiceImpl : CommentService
{
	private DatabaseContext db;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IMapper mapper;
	private readonly IConfiguration configuration;
	private readonly UserServiceAccessor userServiceAccessor;
	public CommentServiceImpl(DatabaseContext db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserServiceAccessor userServiceAccessor)
	{
		this.configuration = configuration;
		this.mapper = mapper;
		_httpContextAccessor = httpContextAccessor;
		this.db = db;
		this.userServiceAccessor = userServiceAccessor;
	}

	public async Task<IActionResult> Create(CommentDTO commentDTO)
	{
		var comment = mapper.Map<Comment>(commentDTO);
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				comment.ParentCommentId = 0;
				comment.AccountId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name));
				db.Comments.Add(comment);
				if(await db.SaveChangesAsync() > 0)
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
			return new BadRequestObjectResult(new {msg = ex.Message});
		}
	}

	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			var data = await db.Comments.FindAsync(id);
			if (data == null)
			{
				return new BadRequestObjectResult(new { msg = "Id does not exist" });
			}
			if (data.AccountId == Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)) || (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name).ToLower()== "Administrator".ToLower())){
			
			
					if (data.ParentCommentId.GetValueOrDefault() == 0)
					{
						var data2 = await db.Comments.Where(x => x.ParentCommentId == data.Id).ToListAsync();
						db.Comments.RemoveRange(data2);
						db.SaveChangesAsync();
						db.Comments.Remove(data);
						db.SaveChangesAsync();
						return new OkObjectResult(new { msg = true });
					}
					else
					{
						db.Comments.Remove(data);
						db.SaveChangesAsync();
						return new OkObjectResult(new { msg = false });
					}
				
				
			}else {
				return new UnauthorizedResult();
				}
			
		}catch(Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> Reply(CommentDTO commentDTO)
	{
		var comment = mapper.Map<Comment>(commentDTO);
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				comment.AccountId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name));
				db.Comments.Add(comment);
				if (await db.SaveChangesAsync() > 0)
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

	public async Task<IActionResult> FindAll(int idTask)
	{
		try { 
			if(await db.Comments.AnyAsync() ==false || await db.Comments.FirstOrDefaultAsync(x=>x.TaskId == idTask) == null)
			{
				return  new BadRequestObjectResult(new { msg = "Data is null" });
			}
			else
			{
				return new OkObjectResult(await db.Comments.Where(x => x.TaskId == idTask).Select( x => new
				{
					id = x.Id,
					taskId = x.TaskId,
					parentComnentId = x.ParentCommentId,
					comment = x.Comment1,
					createDate = x.CreateDate,
					createBy = userServiceAccessor.GetByName2(x.AccountId)
				}).ToListAsync()) ;
			}
		}catch(Exception ex)
		{
			return new BadRequestObjectResult(new {msg = ex.Message});
		}
	}
}
