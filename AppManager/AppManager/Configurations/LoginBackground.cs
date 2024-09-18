/*using Quartz;

namespace Project_2_Web_Api.Configurations;

//[DisallowConcurrentExecution]
public class LoginBackground : IJob
{
	private readonly ILogger<LoginBackground> _logger;

	public LoginBackground(ILogger<LoginBackground> logger)
	{
		_logger = logger;
	}

	public Task Execute(IJobExecutionContext context)
	{
		_logger.LogInformation("{UtcNow}", DateTime.UtcNow);
		return Task.CompletedTask;
	}
}
*/