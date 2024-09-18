using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Helplers;
using Project_2_Web_API.Models;
using System.Globalization;
using System.Security.Claims;

namespace Project_2_Web_Api.Service.Impl;

public class VisitServiceImpl : VisitService
{
	private DatabaseContext db;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IMapper mapper;
	private IConfiguration configuration;
	private UserServiceAccessor userServiceAccessor;
	public VisitServiceImpl(DatabaseContext db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserServiceAccessor userServiceAccessor)
	{
		this.mapper = mapper;
		this.configuration = configuration;
		_httpContextAccessor = httpContextAccessor;
		this.db = db;
		this.userServiceAccessor = userServiceAccessor;
	}

	public async Task<IActionResult> Create(VisitDTO visitDTO)
	{
		var modelState = _httpContextAccessor.HttpContext?.Items["MS_ModelState"] as ModelStateDictionary;
		var visit = mapper.Map<Visit>(visitDTO);
		try
		{
			if (modelState != null && !modelState.IsValid)
			{
				return new BadRequestObjectResult(modelState);
			}
			else
			{
				if(await userServiceAccessor.IsGuest() || await userServiceAccessor.IsDistributor() || await userServiceAccessor.IsSales())
				{
					if (!await userServiceAccessor.CheckPermission("Create new visit plan")) {
						return new UnauthorizedResult();
					}
				}
				visit.Status = "Chưa Viếng Thăm";
				visit.CreateBy = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name));
				var distributor = await db.Distributors.FindAsync(visit.DistributorId);
				if(distributor == null)
				{
					return new BadRequestObjectResult(new {msg = "Id Distributor does not exist"});
				}
				var nameCreateBy = await db.StaffUsers.FindAsync(visit.CreateBy);
				var mailHelper = new MailHelper(configuration);
				var content = "<h2>CDExcellent</h2><br>" +
							"<h2>Dear " + distributor.Name + "!</h2>" +
						"<h3>CDExcellent would like to invite "+distributor.Name+" to attend a visit to the company on "+visit.Calendar.Value.ToString("dd-MM-yyyy")+" at "+visit.Time+".</h3>" +
						"<h3>This visit is intended to "+visit.PurposeOfVisit+ " We would like to have the opportunity to meet, discuss and learn more about"+distributor.Name +"</h3>" +
						"<h2>We look forward to welcoming "+distributor.Name+" to your visit!</h2>" +
						"<h2>Best regards!</h2>" +
						"<h3>Sender's name: "+ nameCreateBy.Fullname+ "</h3>" +
						"<h3>Position: "+nameCreateBy.Position.Name+"</h3>";
				var check = mailHelper.Send(configuration["Gmail:Username"], distributor.Email, "Invitation to attend a company visit", content);
				if (!check)
				{
					return new BadRequestObjectResult(new { msg = "Email sending failed." });
				}
				db.Visits.Add(visit);
				if(await db.SaveChangesAsync() >0 ) {
					return new OkObjectResult(new { msg = true });
				}
				else
				{
					return new BadRequestObjectResult(new { msg = false });
				}
			}
		}
		catch(Exception ex) {

			return new BadRequestObjectResult(new
			{
				error = ex.Message
			});

		}
	}

	public async Task<IActionResult> History()
	{
		try
		{
			if( await db.Visits.AnyAsync() == false)
			{
				return new OkObjectResult(new { msg = "Data is null !" });
			}
			if(await userServiceAccessor.IsGuest() || await userServiceAccessor.IsDistributor())
			{
				return new UnauthorizedResult();
			}
			else
			{
				return new OkObjectResult(
					await db.Visits.Select(x => new
					{
						id = x.Id,
						calendar = x.Calendar,
						time = x.Time,
						purposeOfVisit = x.PurposeOfVisit,
						distributor = new
						{
							id = x.DistributorId,
							name = x.Distributor.Name
						},
						guestOfVisit = x.GuestOfVisit== null? null: new
						{
							id = x.GuestOfVisitId,
							fullname = x.GuestOfVisit.Fullname
						},
						createBy = new
						{
							id = x.CreateBy,
							fullname = x.StaffUser.Fullname,
							position = x.StaffUser.Position.Name
						},
						status = x.Status
					}).ToListAsync()
					) ;
			}
		}catch(Exception ex)
		{
			return new BadRequestObjectResult(new
			{
				msg = ex.Message
			});
		}
	}

	public async Task<IActionResult> Delete(int id)
	{
		
		try
		{
			
			var data = await db.Visits.FindAsync(id);
			if (data == null)
			{
				return new BadRequestObjectResult(new { error = "Id does not exist!" });
			}
			if(await userServiceAccessor.IsSystem())
			{
				data.CreateBy = null;
				data.Distributor = null;
				data.GuestOfVisit = null;
				db.Entry(data).State = EntityState.Modified;
				await db.SaveChangesAsync();
				db.Visits.Remove(await db.Visits.FindAsync(id));
				
				var removeTaskForVisit = await db.TaskForVisit.Where(x => x.VisitId == data.Id).ToListAsync();
				if(removeTaskForVisit != null)
				{
					foreach(var i in removeTaskForVisit)
					{
						db.Comments.Remove(await db.Comments.FirstOrDefaultAsync(x => x.TaskId == i.Id));
						await db.SaveChangesAsync();
					}
					db.TaskForVisit.RemoveRange(removeTaskForVisit);
				}
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
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> Search(DateTime? startDate, DateTime? endDate, string? status, Guid? distributorId)
	{
		try
		{
			using (var dbContext = this.db)
			{
				IQueryable<Visit> query = dbContext.Visits.Include(v => v.Distributor).Include(v => v.GuestOfVisit).Include(v => v.StaffUser);

				if (startDate != null)
				{
					query = query.Where(v => v.Calendar >= startDate);
				}

				if (endDate != null)
				{
					query = query.Where(v => v.Calendar <= endDate);
				}

				if (status != null)
				{
					query = query.Where(v => v.Status == status);
				}

				if (distributorId != null)
				{
					query = query.Where(v => v.DistributorId == distributorId);
				}
				List<Visit> visits = await query.ToListAsync();

				return new OkObjectResult(visits.Select(x => new
				{
					id = x.Id,
					calendar = x.Calendar,
					time = x.Time,
					purposeOfVisit = x.PurposeOfVisit,
					distributor = new
					{
						id = x.DistributorId,
						name = x.Distributor.Name
					},
					guestOfVisit = x.GuestOfVisit == null ? null : new
					{
						id = x.GuestOfVisitId,
						fullname = x.GuestOfVisit.Fullname
					},
					createBy = new
					{
						id = x.CreateBy,
						fullname = x.StaffUser.Fullname
					},
					status = x.Status

				}));
			}
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(new { msg = ex.Message });
		}
	}

	public async Task<IActionResult> FindById(int id)
	{
		try
		{
			if (await db.Visits.AnyAsync() == false)
			{
				return new OkObjectResult(new { msg = "Data is null !" });
			}
			if (await db.Visits.FindAsync(id) == null)
			{
				return new OkObjectResult(new { msg = "Id does not exist !" });
			}
			else
			{
				return new OkObjectResult(
					await db.Visits.Where(x=>x.Id==id).Select(x => new
					{
						id = x.Id,
						calendar = x.Calendar,
						time = x.Time,
						purposeOfVisit = x.PurposeOfVisit,
						distributor = new
						{
							id = x.DistributorId,
							name = x.Distributor.Name
						},
						guestOfVisit = x.GuestOfVisit == null ? null : new
						{
							id = x.GuestOfVisitId,
							fullname = x.GuestOfVisit.Fullname
						},
						createBy = new
						{
							id = x.CreateBy,
							fullname = x.StaffUser.Fullname
						},
						status = x.Status
					}).FirstOrDefaultAsync()
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
			if (await db.Visits.AnyAsync() == false)
			{
				return new OkObjectResult(new { msg = "Data is null !" });
			}
			else
			{
				if(await userServiceAccessor.IsGuest() || await userServiceAccessor.IsDistributor() || await userServiceAccessor.IsSales())
				{
					if (! await userServiceAccessor.CheckPermission("View all visit plans")) 
					{
						return new UnauthorizedResult();
					}
				}
				return new OkObjectResult(
					await db.Visits.Select(x => new
					{
						id = x.Id,
						calendar = x.Calendar,
						time = x.Time,
						purposeOfVisit = x.PurposeOfVisit,
						distributor = new
						{
							id = x.DistributorId,
							name = x.Distributor.Name
						},
						guestOfVisit = x.GuestOfVisit == null ? null : new
						{
							id = x.GuestOfVisitId,
							fullname = x.GuestOfVisit.Fullname
						},
						createBy = new
						{
							id = x.CreateBy,
							fullname = x.StaffUser.Fullname
						},
						status = x.Status
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

	public async Task<IActionResult> FindAllForMe()
	{
		try
		{
			if (await db.Visits.AnyAsync() == false)
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
					return new OkObjectResult(
					await db.Visits.Where(x => x.DistributorId == Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name))).Select(x => new
					{
						id = x.Id,
						calendar = x.Calendar,
						time = x.Time,
						purposeOfVisit = x.PurposeOfVisit,
						distributor = new
						{
							id = x.DistributorId,
							name = x.Distributor.Name
						},
						guestOfVisit = x.GuestOfVisit == null ? null : new
						{
							id = x.GuestOfVisitId,
							fullname = x.GuestOfVisit.Fullname
						},
						createBy = new
						{
							id = x.CreateBy,
							fullname = x.StaffUser.Fullname
						},
						status = x.Status
					}).ToListAsync()
					);
				}
				else if (await userServiceAccessor.IsSales())
				{
					return new OkObjectResult(
					await db.Visits.Where(x => x.GuestOfVisitId == Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name))).Select(x => new
					{
						id = x.Id,
						calendar = x.Calendar,
						time = x.Time,
						purposeOfVisit = x.PurposeOfVisit,
						distributor = new
						{
							id = x.DistributorId,
							name = x.Distributor.Name
						},
						guestOfVisit = x.GuestOfVisit == null ? null : new
						{
							id = x.GuestOfVisitId,
							fullname = x.GuestOfVisit.Fullname
						},
						createBy = new
						{
							id = x.CreateBy,
							fullname = x.StaffUser.Fullname
						},
						status = x.Status
					}).ToListAsync()
					);
				}
				else;
				return new OkObjectResult(
					await db.Visits.Select(x => new
					{
						id = x.Id,
						calendar = x.Calendar,
						time = x.Time,
						purposeOfVisit = x.PurposeOfVisit,
						distributor = new
						{
							id = x.DistributorId,
							name = x.Distributor.Name
						},
						guestOfVisit = x.GuestOfVisit == null ? null : new
						{
							id = x.GuestOfVisitId,
fullname = x.GuestOfVisit.Fullname
						},
						createBy = new
						{
							id = x.CreateBy,
							fullname = x.StaffUser.Fullname
						},
						status = x.Status
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

	public async Task<IActionResult> Detail(int id)
	{
		try
		{
			if (await db.Visits.AnyAsync() == false)
			{
				return new OkObjectResult(new { msg = "Data is null !" });
			}
			if (await db.Visits.FindAsync(id) == null)
			{
				return new OkObjectResult(new { msg = "Id does not exist !" });
			}
			else
			{
				return new OkObjectResult(
					await db.Visits.Where(x => x.Id == id).Select(x => new
					{
						id = x.Id,
						calendar = x.Calendar,
						time = x.Time,
						purposeOfVisit = x.PurposeOfVisit,
						distributor = new
						{
							id = x.DistributorId,
							name = x.Distributor.Name
						},
						guestOfVisit = x.GuestOfVisit == null ? null : new
						{
							id = x.GuestOfVisitId,
							fullname = x.GuestOfVisit.Fullname
						},
						createBy = new
						{
							id = x.CreateBy,
							fullname = x.StaffUser.Fullname
						},
						status = x.Status
					}).FirstOrDefaultAsync()
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

	public async Task<IActionResult> HistoryVisitingSchedule()
	{
		try
		{
			if (await db.Visits.AnyAsync() == false)
			{
				return new OkObjectResult(new { msg = "Data is null !" });
			}
			else
			{
				return new OkObjectResult(
					await db.Visits.Where(x=>
					(x.Status.ToLower()!="Đã Viếng thăm".ToLower() && x.Calendar >= DateTime.Now) ||( x.Status.ToLower()!="Đã Hủy".ToLower() && x.Calendar >= DateTime.Now))
					.Select(x => new
					{
						id = x.Id,
						calendar = x.Calendar,
						time = x.Time,
						purposeOfVisit = x.PurposeOfVisit,
						distributor = new
						{
							id = x.DistributorId,
							name = x.Distributor.Name
						},
						guestOfVisit = x.GuestOfVisit == null ? null : new
						{
							id = x.GuestOfVisitId,
							fullname = x.GuestOfVisit.Fullname
						},
						createBy = new
						{
							id = x.CreateBy,
							fullname = x.StaffUser.Fullname,
							position = x.StaffUser.Position.Name
						},
						status = x.Status
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
}
