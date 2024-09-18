using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Helplers;
using Project_2_Web_Api.Models;
using Project_2_Web_API.Models;
using System.Diagnostics;


namespace Project_2_Web_Api.Service.Impl;

public class NotificationServiceImpl : NotificationService
{

	private DatabaseContext db;
	private readonly UserServiceAccessor userServiceAccessor;
	private IConfiguration configuration;
	private DatabaseContext databaseContext;
	private readonly IMapper mapper;
	private readonly IHttpContextAccessor httpContextAccessor;
	public NotificationServiceImpl(
		DatabaseContext db,
		IMapper mapper,
		UserServiceAccessor userServiceAccessor,
		IHttpContextAccessor httpContextAccessor,
		UserServiceAccessor serviceAccessor,
		IConfiguration configuration)
	{
		this.userServiceAccessor = userServiceAccessor;
		this.httpContextAccessor = httpContextAccessor;
		this.mapper = mapper;
		this.db = db;
		this.configuration = configuration;
		this.userServiceAccessor = userServiceAccessor;
	}

	public NotificationServiceImpl()
	{
	}

	public NotificationServiceImpl(DatabaseContext databaseContext)
	{
		this.databaseContext = databaseContext;
	}

	public async Task<IActionResult> Create(NotificationDTO notificationDTO)
	{
		var notification = mapper.Map<Notification>(notificationDTO);
		var modelState = httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		try
		{
			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if (await userServiceAccessor.CheckPermission("Create Notification") || await userServiceAccessor.IsSystem())
				{
					notification.CreateBy = await userServiceAccessor.GetById();
					var count = 0;
					foreach (var i in notificationDTO.Receiver)
					{
						var id = Guid.Parse(i);

						if (await db.Users.FindAsync(id) != null)
						{
							notification.User.Add(await db.Users.FindAsync(id));
						}
						else if (await db.StaffUsers.FindAsync(id) != null)
						{
							notification.StaffUsers.Add(await db.StaffUsers.FindAsync(id));
						}
						else if (await db.Distributors.FindAsync(id) != null)
						{
							notification.Distributors.Add(await db.Distributors.FindAsync(id));
						}
						else
						{
							count++;
						}
					}
					if (count > 0)
					{
						return new BadRequestObjectResult(new { msg = "A total of " + count + " people could not send" });
					}
					else
					{

						db.Notifications.Add(notification);
						if (await db.SaveChangesAsync() > 0)
						{
							var count2 = 0;
							List<string> emailNoSend = new List<string>();
							if (notification.User != null)
							{
								foreach (var item in notification.User)
								{
									var mailHelper = new MailHelper(configuration);
									var check = mailHelper.Send(configuration["Gmail:Username"], item.Email, "Notidication		CDExcellent", MailHelper.HtmlNotification(item.FullName, notification.Content));
									if (!check)
									{
										count2++;
										emailNoSend.Add(item.Email);
									}
								}
							}
							if (notification.StaffUsers != null)
							{
								foreach (var item in notification.StaffUsers)
								{
									var mailHelper = new MailHelper(configuration);
									var check = mailHelper.Send(configuration["Gmail:Username"], item.Email, "Notidication		CDExcellent", MailHelper.HtmlNotification(item.Fullname, notification.Content));
									if (!check)
									{
										count2++;
										emailNoSend.Add(item.Email);
									}
								}
							}
							if (notification.Distributors != null)
							{
								foreach (var item in notification.Distributors)
								{
									var mailHelper = new MailHelper(configuration);
									var check = mailHelper.Send(configuration["Gmail:Username"], item.Email, "Notidication	 CDExcellent", MailHelper.HtmlNotification(item.Name, notification.Content));
									if (!check)
									{
										count2++;
										emailNoSend.Add(item.Email);
									}
								}
							}
							if (count2 > 0)
							{
								return new OkObjectResult(new { msg = "A total of " + count2 + " people could not send.", list = emailNoSend.ToList() });
							}
							return new OkObjectResult(new { msg = "send success" });
						}
						else
						{
							return new BadRequestObjectResult(new { error = "send notification fail" });
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
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> FindAll()
	{
		try
		{
			if(await db.Notifications.AnyAsync() == false)
			{
				return new BadRequestObjectResult(new { msg = "data is null" });
			}
			if (await userServiceAccessor.IsSystem())
			{
				return new OkObjectResult( await db.Notifications.Select(  x => new
				{
					id = x.Id,
					title = x.Title,
					content = x.Content,
					createBy  =  userServiceAccessor.GetByName2(x.CreateBy),
					receiverUser = x.User == null? null: x.User.Select(i => new
					{
						id = i.Id,
						fullname = i.FullName,
						email = i.Email
					}),
					receiverStaffUser = x.StaffUsers == null ? null : x.StaffUsers.Select(i => new
					{
						id = i.Id,
						fullname = i.Fullname,
						email = i.Email
					}),
					receiverDistributor = x.Distributors == null ? null : x.Distributors.Select(i => new
					{
						id = i.Id,
						name = i.Name,
						email = i.Email
					})
				}).ToListAsync());
			}
			else
			{
				return new UnauthorizedResult();
			}
			
		}catch(Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> FindAllForMe()
	{
		try
		{
			if (await db.Notifications.AnyAsync() == false)
			{
				return new BadRequestObjectResult(new { msg = "data null" });
			}
			var outPut = new List<Notification>();
			var data = await db.Notifications.ToListAsync();
			if (await userServiceAccessor.IsGuest())
			{
				foreach(var i in data)
				{
					foreach(var j in i.User)
					{
						if(await userServiceAccessor.GetById() == j.Id)
						{
							outPut.Add(i);
						}
					}
					break;
				}
				return new OkObjectResult(outPut.Select(x => new
				{
					id = x.Id,
					title = x.Title,
					content = x.Content,
					createBy = userServiceAccessor.GetByName(x.CreateBy)
				}).ToList());
			}
			else if (await userServiceAccessor.IsDistributor())
			{
				foreach (var i in data)
				{
					foreach (var j in i.Distributors)
					{
						if (await userServiceAccessor.GetById() == j.Id)
						{
							outPut.Add(i);
						}
					}
					break;
				}
				return new OkObjectResult(outPut.Select(x => new
				{
					id = x.Id,
					title = x.Title,
					content = x.Content,
					createBy = userServiceAccessor.GetByName(x.CreateBy)
				}).ToList());
			}
			else
			{
				foreach (var i in data)
				{
					foreach (var j in i.StaffUsers)
					{
						if (await userServiceAccessor.GetById() == j.Id)
						{
							outPut.Add(i);
						}
					}
					break;
				}
				return new OkObjectResult(outPut.Select(x => new
				{
					id = x.Id,
					title = x.Title,
					content = x.Content,
					createBy = userServiceAccessor.GetByName(x.CreateBy)
				}).ToList());
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
			if (await db.Notifications.FindAsync(id) == null)
			{
				return new BadRequestObjectResult(new { msg = "id does not exist!" });
			}
			if(await userServiceAccessor.IsSystem())
			{
				db.Notifications.Remove(await db.Notifications.FindAsync(id));
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

	public  async void SendAutoNotificationVisit()
	{
		try
		{
			var data = await db.Visits.Where(x => (x.Calendar.Value.Date == DateTime.Now.Date) && x.Status.ToLower() == "Chưa Thăm Viếng".ToLower()).ToListAsync();
			if (data != null)
			{
				foreach (var i in data)
				{
					string content = "On " + i.Calendar + ", " + i.Time + " you are scheduled to visit our company. We are very honored and pleased to welcome you.";
					var mailHelper = new MailHelper(configuration);
					var check = mailHelper.Send(configuration["Gmail:Username"], i.Distributor.Email, "Notidication	CDExcellent", MailHelper.HtmlNotification(i.Distributor.Name, content));
					string content2 = "On " + i.Calendar + ", " + i.Time + " you are scheduled to visit our company. We are very honored and pleased to welcome you.";
					var mailHelper1 = new MailHelper(configuration);
					var check1 = mailHelper1.Send(configuration["Gmail:Username"], i.GuestOfVisit.Email, "Notidication CDExcellent", MailHelper.HtmlNotification(i.GuestOfVisit.Fullname, content));

				}
			}

		}
		catch(Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}
	}
}
