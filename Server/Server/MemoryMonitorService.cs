
namespace Server;

public class MemoryMonitorService : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		//To verify that the memory usage isn't due to the GC not running
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(30000, stoppingToken);
			Console.WriteLine("Running GC");
			Console.WriteLine("TotalMemory:" + GC.GetTotalMemory(true));
		}
	}
}
