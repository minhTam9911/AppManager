/*using Quartz;

namespace Project_2_Web_Api.Configurations;

public static class DependencyInjectton
{
	public static void AddInfrastructrue(this IServiceCollection services)
	{
		services.AddQuartz(option =>
		{
			
			option.UseMicrosoftDependencyInjectionJobFactory();
		});
		services.AddQuartzHostedService(option=>option.WaitForJobsToComplete = true);
		services.ConfigureOptions<LoginBackgroundJobSetup>();
	}
}
*/