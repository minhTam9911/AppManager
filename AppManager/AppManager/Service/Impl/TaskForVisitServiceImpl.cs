using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Helpers;
using Project_2_Web_API.Models;
using System;
using System.Security.Claims;

namespace Project_2_Web_Api.Service.Impl;

public class TaskForVisitServiceImpl : TaskForVisitService
{

	private DatabaseContext db;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IMapper mapper;
	private IConfiguration configuration;
	private UserServiceAccessor userServiceAccessor;
	private IWebHostEnvironment webHostEnvironment;
	public TaskForVisitServiceImpl(DatabaseContext db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserServiceAccessor userServiceAccessor, IWebHostEnvironment webHostEnvironment)
	{
		this.mapper = mapper;
		this.configuration = configuration;
		_httpContextAccessor = httpContextAccessor;
		this.db = db;
		this.userServiceAccessor = userServiceAccessor;
		this.webHostEnvironment = webHostEnvironment;
	}

	public  async Task<IActionResult> Create(TaskForVisitDTO taskForVisitDTO)
	{
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		var taskForVisit = mapper.Map<TaskForVisit>(taskForVisitDTO);
		try
		{
			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if(await db.Visits.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == taskForVisit.VisitId) == null)
				{
					return new OkObjectResult(new { msg = "Id Visit does not exist" });
				}
				taskForVisit.CreateBy = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name));
				taskForVisit.Status = "Đã Giao Việc";
				db.TaskForVisit.Add(taskForVisit);
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
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> DetailTask(int id)
	{
		try
		{
			if (await db.TaskForVisit.AnyAsync() == false)
			{
				return new OkObjectResult(new { msg = "Data is null !" });
			}
			else
			{
				if (await userServiceAccessor.IsGuest())
				{
					return new UnauthorizedResult();
				}
				else if (await userServiceAccessor.IsDistributor())
				{
					return new UnauthorizedResult();
				}
				else if (await userServiceAccessor.IsSales())
				{
					return new OkObjectResult(
					await db.TaskForVisit.Where(x => x.Id == id).Select(x => new
					{
						id = x.Id,
						title = x.Title,
						visitId = x.VisitId,
						assignedStaffUser = new
						{
							id = x.AssignedStaffUserId,
							name = x.StaffUserAssignee.Fullname
						},
						reportStaffUser = new
						{
							id = x.ReportingStaffUserId,
							name = x.StaffUserReposter.Fullname
						},
						description = x.Description,
						startDate = x.StartDate,
						endDate = x.EndDate,

					}).ToListAsync()
					);
				}
				else;
				return new OkObjectResult(
					await db.TaskForVisit.Where(x => x.Id == id).Select(x => new
					{
						id = x.Id,
						title = x.Title,
						visitId = x.VisitId,
						assignedStaffUser = new
						{
							id = x.AssignedStaffUserId,
							name = x.StaffUserAssignee.Fullname
						},
						reportStaffUser = new
						{
							id = x.ReportingStaffUserId,
							name = x.StaffUserReposter.Fullname
						},
						photoPathAssigned = x.PhotoPathAssigned == null? null : x.PhotoPathAssigned.Select(i => new
						{
							fileName = configuration["BaseUrl"] + "TaskForVisit/"+i.Path
						}),
						photoPathReporting = x.PhotoPathReporting == null ? null : x.PhotoPathReporting.Select(i => new
						{
							fileName = configuration["BaseUrl"] + "TaskForVisit/" + i.Path
						}),
						description = x.Description,
						startDate = x.StartDate,
						endDate = x.EndDate,

					}).ToListAsync()
					);
			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new
			{
				msg = ex.Message
			});
		}
	}

	public async Task<IActionResult> FindAll()
	{
		try
		{
			if (await db.TaskForVisit.AnyAsync() == false)
			{
				return new OkObjectResult(new { msg = "Data is null !" });
			}
			else
			{
				if (await userServiceAccessor.IsGuest())
				{
					return new UnauthorizedResult();
				}
				else if (await userServiceAccessor.IsDistributor())
				{
					return new UnauthorizedResult();
				}
				else if (await userServiceAccessor.IsSales())
				{
					return new OkObjectResult(
					await db.TaskForVisit.Where(x => x.AssignedStaffUserId == Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name))).Select(x => new
					{
						id = x.Id,
						title = x.Title,
						visitId = x.VisitId,
						assignedStaffUser = new
						{
							id = x.AssignedStaffUserId,
							name = x.StaffUserAssignee.Fullname
						},
						reportStaffUser = new
						{
							id = x.ReportingStaffUserId,
							name = x.StaffUserReposter.Fullname
						},
						description = x.Description,
						startDate = x.StartDate,
						endDate = x.EndDate,
						photoPathReporting = x.PhotoPathReporting == null? null : x.PhotoPathReporting.Select(x => new
						{
							file = configuration["BaseUrl"] + "TaskForVisit/" + x.Path
						}),
						photoPathAssigned = x.PhotoPathAssigned == null ? null : x.PhotoPathAssigned.Select(x => new
						{
							file = configuration["BaseUrl"] + "TaskForVisit/" + x.Path
						})
					}).ToListAsync()
					);
				}
				else;
				return new OkObjectResult(
					await db.TaskForVisit.Select(x => new
					{
						id = x.Id,
						title = x.Title,
						visitId = x.VisitId,
						assignedStaffUser = new
						{
							id = x.AssignedStaffUserId,
							name = x.StaffUserAssignee.Fullname
						},
						reportStaffUser = new
						{
							id = x.ReportingStaffUserId,
							name = x.StaffUserReposter.Fullname
						},
						description = x.Description,
						startDate = x.StartDate,
						endDate = x.EndDate,

					}).ToListAsync()
					);
			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new
			{
				msg = ex.Message
			});
		}
	}

	public async Task<IActionResult> FindById(int id)
	{
		try
		{
			if (await db.TaskForVisit.AnyAsync() == false || await db.TaskForVisit.FindAsync(id) == null )
			{
				return new OkObjectResult(new { msg = "Data is null !" });
			}
			else
			{
				if (await userServiceAccessor.IsGuest())
				{
					return new UnauthorizedResult();
				}
				else if (await userServiceAccessor.IsDistributor())
				{
					return new UnauthorizedResult();
				}
				else if (await userServiceAccessor.IsSales())
				{
					return new OkObjectResult(
					await db.TaskForVisit.Where(x => x.Id ==id).Select(x => new
					{
						id = x.Id,
						title = x.Title,
						visitId = x.VisitId,
						assignedStaffUser = new
						{
							id = x.AssignedStaffUserId,
							name = x.StaffUserAssignee.Fullname
						},
						reportStaffUser = new
						{
							id = x.ReportingStaffUserId,
							name = x.StaffUserReposter.Fullname
						},
						description = x.Description,
						startDate = x.StartDate,
						endDate = x.EndDate,
						photoPathReporting = x.PhotoPathReporting == null ? null : x.PhotoPathReporting.Select(x => new
						{
							file = configuration["BaseUrl"] + "TaskForVisit/" + x.Path
						}),
						photoPathAssigned = x.PhotoPathAssigned == null ? null : x.PhotoPathAssigned.Select(x => new
						{
							file = configuration["BaseUrl"] + "TaskForVisit/" + x.Path
						})
					}).ToListAsync()
					);
				}
				else;
				return new OkObjectResult(
					await db.TaskForVisit.Where(x => x.Id == id).Select(x => new
					{
						id = x.Id,
						title = x.Title,
						visitId = x.VisitId,
						assignedStaffUser = new
						{
							id = x.AssignedStaffUserId,
							name = x.StaffUserAssignee.Fullname
						},
						reportStaffUser = new
						{
							id = x.ReportingStaffUserId,
							name = x.StaffUserReposter.Fullname
						},
						description = x.Description,
						startDate = x.StartDate,
						endDate = x.EndDate,

					}).ToListAsync()
					);
			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new
			{
				msg = ex.Message
			});
		}
	}

	public async Task<IActionResult> TaskForMe()
	{
		try
		{
			if (await db.TaskForVisit.AnyAsync() == false)
			{
			return new OkObjectResult(new { msg = "Data is null !" });
			}
			else
			{
				if (await userServiceAccessor.IsGuest())
				{
					return new UnauthorizedResult();
				}
				else if (await userServiceAccessor.IsDistributor())
				{
					return new UnauthorizedResult();
				}
				else if (await userServiceAccessor.IsSales())
				{
					return new OkObjectResult(
					await db.TaskForVisit.Where(x => x.AssignedStaffUserId == Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name))).Select(x => new
					{
						id = x.Id,
						title = x.Title,
						visitId = x.VisitId,
						assignedStaffUser = new
						{
							id = x.AssignedStaffUserId,
							name = x.StaffUserAssignee.Fullname
						},
						reportStaffUser = new
						{
							id = x.ReportingStaffUserId,
							name = x.StaffUserReposter.Fullname
						},
						description = x.Description,
						startDate = x.StartDate,
						endDate = x.EndDate,

					}).ToListAsync()
					);
				}
				else
				{
					return new OkObjectResult(
						await db.TaskForVisit.Where(x => x.AssignedStaffUserId == Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name))).Select(x => new
						{
							id = x.Id,
							title = x.Title,
							visitId = x.VisitId,
							assignedStaffUser = new
							{
								id = x.AssignedStaffUserId,
								name = x.StaffUserAssignee.Fullname
							},
							reportStaffUser = new
							{
								id = x.ReportingStaffUserId,
								name = x.StaffUserReposter.Fullname
							},
							description = x.Description,
							startDate = x.StartDate,
							endDate = x.EndDate,

						}).ToListAsync()
						);
				}
			}
	}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new
			{
				msg = ex.Message
});
		}
	}

	public async  Task<IActionResult> UploadFile(int id,IFormFile[] fileAssigned, IFormFile[] fileReport)
	{
		try
		{
			var data = await db.TaskForVisit.FindAsync(id);

			if (data == null)
			{
				return new BadRequestObjectResult(new { error = "Id does not exist" });
			}
			foreach (var file in fileReport)
			{
				var fileName = FileHelper.generateFileName(file.FileName)!;
				var path = Path.Combine(webHostEnvironment.WebRootPath, "TaskForVisit", fileName);
				using (var fileStream = new FileStream(path, FileMode.Create))
				{
					file.CopyTo(fileStream);
				}
				data.PhotoPathReporting.Add(new PhotoPathReporting { Path = fileName }) ;
			}
			foreach (var file in fileAssigned)
			{
				var fileName = FileHelper.generateFileName(file.FileName);
				var path = Path.Combine(webHostEnvironment.WebRootPath, "TaskForVisit", fileName);
				using (var fileStream = new FileStream(path, FileMode.Create))
				{
					file.CopyTo(fileStream);
				}
				data.PhotoPathAssigned.Add(new PhotoPathAssigned { Path = fileName});
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
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> UploadFileTaskForMe(int id, IFormFile[] fileReport, string status)
	{
		try
		{
			var data = await db.TaskForVisit.FindAsync(id);
			if (data == null)
			{
				return  new BadRequestObjectResult(new { msg = "Id does not exist" });
			}
			foreach (var file in fileReport)
			{
				var fileName = FileHelper.generateFileName(file.FileName);
				var path = Path.Combine(webHostEnvironment.WebRootPath, "TaskForVisit", fileName);
				using (var fileStream = new FileStream(path, FileMode.Create))
				{
					file.CopyTo(fileStream);
				}
				data.PhotoPathReporting.Add(new PhotoPathReporting { Path = fileName});
			}
			data.Status = status;
			db.Entry(data).State = EntityState.Modified;
			if(await db.SaveChangesAsync() > 0)
			{
				return new OkObjectResult(new { msg =true });
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
