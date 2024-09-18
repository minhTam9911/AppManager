/*using Microsoft.Extensions.Options;
using Project_2_Web_API.Models;
using Quartz;

namespace Project_2_Web_Api.Configurations;

public class LoginBackgroundJobSetup : IConfigureOptions<QuartzOptions>
{
	private readonly DatabaseContext databaseContext;
	public void Configure(QuartzOptions options)
	{

		var jobKey = JobKey.Create(nameof(LoginBackground));
		options.AddJob<LoginBackground>(jobBuilder=>jobBuilder.WithIdentity(jobKey))
			.AddTrigger(trigger => trigger.ForJob(jobKey)
			.WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever()));
	}
}
*/