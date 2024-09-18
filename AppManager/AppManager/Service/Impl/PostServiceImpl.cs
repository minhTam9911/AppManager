using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Helpers;
using Project_2_Web_API.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Project_2_Web_Api.Service.Impl;

public class PostServiceImpl : PostService
{
	private DatabaseContext db;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IMapper mapper;
	private readonly UserServiceAccessor userServiceAccessor;
	private IWebHostEnvironment webHostEnvironment;
	private IConfiguration configuration;
	public PostServiceImpl(DatabaseContext db,
		IHttpContextAccessor httpContextAccessor,
		IMapper mapper,
		UserServiceAccessor userServiceAccessor,
		IConfiguration configuration,
		IWebHostEnvironment webHostEnvironment
		)
	{
		this.db = db;
		this.mapper = mapper;
		_httpContextAccessor = httpContextAccessor;
		this.userServiceAccessor = userServiceAccessor;
		this.webHostEnvironment = webHostEnvironment;
		this.configuration = configuration;
}

	public async Task<IActionResult> Create(PostDTO postDTO)
	{
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		var post = mapper.Map<Post>(postDTO);
		try
		{

			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if (await userServiceAccessor.CheckPermission("Create new article") || await userServiceAccessor.IsSystem())
				{
					
					
					post.CreateDate = DateTime.Now;
					post.CreateBy = await userServiceAccessor.GetById();
					post.IsStatus = false;
					post.FilePath = "no-image.jpg";
					db.Posts.Add(post);
					if (await db.SaveChangesAsync() > 0)
					{
						return new OkObjectResult(new { msg = true, id = post.Id });
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
		}catch(Exception ex)
		{
			return new BadRequestObjectResult(new {msg = ex.Message});
		}
	}

	public async Task<IActionResult> Update(int id, PostDTO postDTO)
	{
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		var post = mapper.Map<Post>(postDTO);
		try
		{

			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if (await userServiceAccessor.CheckPermission("Update article detail") || await userServiceAccessor.IsSystem())
				{
					var data = await db.Posts.FindAsync(id);
					if(data == null)
					{
						return new BadRequestObjectResult(new { msg = "Id does not exist" });
					}
					if(data.CreateBy != await userServiceAccessor.GetById())
					{
						return new UnauthorizedResult();
					}
					data.Title = post.Title;
					data.ShortDescription = post.ShortDescription;
					post.CreateBy = await userServiceAccessor.GetById();
					post.IsStatus = false;
					data.PathOfTheArticle = post.PathOfTheArticle;

					db.Entry(data).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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

	public async Task<IActionResult> Search(string keyword)
	{
		try
		{
			var data = await db.Posts.Where(
				x => x.Title.ToLower().Contains(keyword.ToLower())
				|| x.ShortDescription.ToLower().Contains(keyword.ToLower())
				)
				.ToListAsync();
			if(data == null)
			{
				return new BadRequestObjectResult(new { msg = "Data is null" });
			}
			else
			{
				return new OkObjectResult(await db.Posts.Where(
				x => x.Title.ToLower().Contains(keyword.ToLower())
				|| x.ShortDescription.ToLower().Contains(keyword.ToLower())
				).Select(x => new
				{
					id = x.Id,
					title = x.Title,
					shortDescription = x.ShortDescription,
					createDate = x.CreateDate,
					createBy = x.StaffUser.Fullname,
					filePath = configuration["BaseUrl"] + "Post/" + x.FilePath,
					pathOfTheArticle = x.PathOfTheArticle,
					status = x.IsStatus
				}).ToListAsync());
			}
		}catch(Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> UpdateStatus(int id,bool status)
	{
		try
		{
				if (await userServiceAccessor.CheckPermission("Publish existing article") || await userServiceAccessor.IsSystem())
				{
				
					var data = await db.Posts.FindAsync(id);
					if (data == null)
					{
						return new BadRequestObjectResult(new { msg = "Id does not exist" });
					}
					if (data.CreateBy != await userServiceAccessor.GetById())
					{
						return new UnauthorizedResult();
					}
					
					data.IsStatus = status;
					db.Entry(data).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
					if (await db.SaveChangesAsync() > 0)
					{
						return new OkObjectResult(new { msg = true });
					}
					else
					{
						return new BadRequestObjectResult(new { msg =false });
					}

			}
			else
			{
				return new UnauthorizedResult();
			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { error = ex.Message });
		}
	}

	public async Task<IActionResult> FindAll()
	{
		try
		{
			if (await db.Posts.AnyAsync() == false)
			{
				return new BadRequestObjectResult(new { msg = "Data is null" });
			}
			else
			{
				return new OkObjectResult(await db.Posts.Where(
				x => x.IsStatus == true
				).Select(x => new
				{
					id = x.Id,
					title = x.Title,
					shortDescription = x.ShortDescription,
					createDate = x.CreateDate,
					createBy = x.StaffUser.Fullname,
					filePath = configuration["BaseUrl"] + "Post/" + x.FilePath,
					pathOfTheArticle = x.PathOfTheArticle,
					status = x.IsStatus
				}).ToListAsync());
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
			var data = await db.Posts.FindAsync(id);
			if (data == null)
			{
				return new BadRequestObjectResult(new { msg = "Id does not exist" });
			}
			else if (data.CreateBy != await userServiceAccessor.GetById() && !await userServiceAccessor.IsSystem())
			{
				return new UnauthorizedResult();
			}
			else if(await userServiceAccessor.CheckPermission("Remove unpublish articles") || await userServiceAccessor.IsSystem()) {
				var path = Path.Combine(webHostEnvironment.WebRootPath, "Post", data.FilePath);
				File.Delete(path);
				db.Posts.Remove(data);
				if(await db.SaveChangesAsync() > 0)
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
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> DeleteRange(int[] id)
	{
		try
		{
			if (await userServiceAccessor.CheckPermission("Remove unpublish articles") || await userServiceAccessor.IsSystem())
			{

				var count = 0;
				foreach (var item in id)
				{
					var data = await db.Posts.FindAsync(item);
					if (data == null)
					{
						return new BadRequestObjectResult(new { msg = "Id does not exist" });
					}
					else if (data.CreateBy != await userServiceAccessor.GetById() && !await userServiceAccessor.IsSystem())
					{
						return new UnauthorizedResult();
					}
					else
					{

						var path = Path.Combine(webHostEnvironment.WebRootPath, "Post", data.FilePath);
						File.Delete(path);
						db.Posts.Remove(data);
						if (await db.SaveChangesAsync() > 0)
						{
							count++;
						}
						else;

					}
				}
				if (count == id.Length)
				{
					return new OkObjectResult(new { msg = true });
				}
				else
				{
					return new OkObjectResult(new { msg = false });
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

	public async Task<IActionResult> FindAllForMe()
	{
		try
		{
			if (await db.Posts.AnyAsync() == false)
			{
				return new BadRequestObjectResult(new { msg = "Data is null" });
			}
			else
			{
				var id = await userServiceAccessor.GetById();
				return new OkObjectResult(await db.Posts.Where(
				x => x.CreateBy == id
				).Select(x => new
				{
					id = x.Id,
					title = x.Title,
					shortDescription = x.ShortDescription,
					createDate = x.CreateDate,
					createBy = x.StaffUser.Fullname,
					filePath = configuration["BaseUrl"] + "Post/" + x.FilePath,
					pathOfTheArticle = x.PathOfTheArticle,
					status = x.IsStatus
				}).ToListAsync());
			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> Upload(int id, IFormFile filePath)
	{
		try
		{
			if (await userServiceAccessor.CheckPermission("Update article detail") || await userServiceAccessor.CheckPermission("Create article detail") || await userServiceAccessor.IsSystem())
			{
				var data = await db.Posts.FindAsync(id);
				if (data == null)
				{
					return new BadRequestObjectResult(new { msg = "Id does not exist" });
				}
				if (data.CreateBy != await userServiceAccessor.GetById())
				{
					return new UnauthorizedResult();
				}
				if (filePath == null)
				{
					return new BadRequestObjectResult(new { msg = "file failed" });
				}
				if (!FileHelper.checkFile(filePath))
				{
					return new BadRequestObjectResult(new { msg = "file Invalid" });
				}
				var path = Path.Combine(webHostEnvironment.WebRootPath, "Post", data.FilePath);
				if (data.FilePath != "no-image.jpg")
				{
					File.Delete(path);
				}
				var fileName = FileHelper.generateFileName(filePath.FileName);
				var path2 = Path.Combine(webHostEnvironment.WebRootPath, "Post", fileName);
				using (var fileStream = new FileStream(path2, FileMode.Create))
				{
					filePath.CopyTo(fileStream);
				}
				data.FilePath = fileName;
				db.Entry(data).State = EntityState.Modified;
				if(await db.SaveChangesAsync() > 0){
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
		}catch(Exception ex)
		{
			return new BadRequestObjectResult(new {msg = ex.Message});
		}
		
	}

	
}
