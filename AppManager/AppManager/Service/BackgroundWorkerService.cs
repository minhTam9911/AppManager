using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.Helpers;
using Project_2_Web_Api.Helplers;
using Project_2_Web_API.Models;
using System.Threading;

namespace Project_2_Web_Api.Service;

public class BackgroundWorkerService : BackgroundService
{
	private readonly ILogger<BackgroundWorkerService> logger;
	private IServiceProvider service;
	private readonly IConfiguration configuration;
	public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IConfiguration configuration ,IServiceProvider service)
	{
		this.logger = logger;
		this.service = service;
		this.configuration = configuration;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("Service Start");

	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("stop Start");
		return Task.CompletedTask;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{

			var startTime = DateTimeOffset.Now.ToUniversalTime().AddDays(1).AddHours(0).AddMinutes(0).AddSeconds(0);
			logger.LogInformation("Word Runing at: {time}", DateTimeOffset.Now);
			using (var scope = service.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
				var data = await db.Visits.Where(x => (x.Calendar.Value.Date == DateTime.Now.Date) && x.Status.ToLower() == "Chưa Viếng Thăm".ToLower()).ToListAsync();
					if (data != null || data.Count >0)
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

			await Task.Delay(24*60*60*1000, stoppingToken);

		}
	}
}
