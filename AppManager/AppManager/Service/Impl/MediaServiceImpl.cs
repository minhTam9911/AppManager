using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Project_2_Web_Api.Helpers;
using Project_2_Web_API.Models;
using System;
using System.Web.Http.ModelBinding;
using static System.Net.Mime.MediaTypeNames;

namespace Project_2_Web_Api.Service.Impl;

public class MediaServiceImpl : MediaService
{

	private DatabaseContext db;
	private readonly UserServiceAccessor userServiceAccessor;
	private IWebHostEnvironment webHostEnvironment;
	private IConfiguration configuration;
	private IHttpContextAccessor httpContextAccessor;
	public MediaServiceImpl(DatabaseContext db,
		IHttpContextAccessor httpContextAccessor,
		IMapper mapper,
		UserServiceAccessor userServiceAccessor,
		IConfiguration configuration,
		IWebHostEnvironment webHostEnvironment
		)
	{
		this.db = db;
		this.configuration = configuration;
		this.userServiceAccessor = userServiceAccessor;
		this.webHostEnvironment = webHostEnvironment;
		this.httpContextAccessor = httpContextAccessor;
	}
	public async Task<IActionResult> Create(IFormFile filePath)
	{
		try
		{
			if (filePath == null)
			{
				return new BadRequestObjectResult(new { msg = "file failed" });
			}
			if (!FileHelper.checkFileMedia(filePath))
			{
				return new BadRequestObjectResult(new { msg = "file Invalid" });
			}
			var media = new Media();
			var fileName = FileHelper.generateFileName(filePath.FileName);
			var path = Path.Combine(webHostEnvironment.WebRootPath, "Media", fileName);
			using (var fileStream = new FileStream(path, FileMode.Create))
			{
				filePath.CopyTo(fileStream);
			}
			media.FilePath = fileName;
			media.CreateDate = DateTime.Now;
			media.CreateBy = await userServiceAccessor.GetById();
			db.Medias.Add(media);
			if (await db.SaveChangesAsync() > 0)
			{
				return new OkObjectResult(new { msg = true});
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
			var data = await db.Medias.FindAsync(id);
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
				var path = Path.Combine(webHostEnvironment.WebRootPath, "Media", data.FilePath);
				File.Delete(path);
				db.Medias.Remove(data);
				if (await db.SaveChangesAsync() > 0)
				{
					return new OkObjectResult(new { msg = true });
				}
				else
				{
					return new BadRequestObjectResult(new { msg = true });
				}

			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> FindAllForMe()
	{
		try {
			var id = await userServiceAccessor.GetById();
			return new OkObjectResult(await db.Medias.Where(x=>x.CreateBy == id).Select(x => new
			{
				id = x.Id,
				filePath = configuration["BaseUrl"]+"Media/"+ x.FilePath,
				createDate = x.CreateDate
			}).ToListAsync());
		}catch(Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> FindAll()
	{
		try
		{
			var httpClientFactory = httpContextAccessor.HttpContext.RequestServices.GetService<IHttpClientFactory>();
			var id = await userServiceAccessor.GetById();
			return new OkObjectResult(await db.Medias.Select(x => new
			{
				id = x.Id,
				filePath = configuration["BaseUrl"]+"Media/" + x.FilePath,
				createDate = x.CreateDate,
				
		}).ToListAsync());
		}
		catch (Exception ex)	
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}
}
