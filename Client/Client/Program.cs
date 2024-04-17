// See https://aka.ms/new-console-template for more information
using System.Net.WebSockets;

Console.WriteLine("Hello, World!");

var tasks = new List<Task>();
int connected = 0;
long sent = 0;

for (var i = 0; i < 100; i++)
{
	tasks.Add(Task.Run(async () =>
	{
		using var ws = new ClientWebSocket();

		try
		{
			await ws.ConnectAsync(new Uri("ws://localhost:5006/WebSocket"), CancellationToken.None);

			Interlocked.Increment(ref connected);
			Console.WriteLine($"CC Connected: {connected}");
			var buffer = new byte[1];
			while (true)
			{
				await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
				var s = Interlocked.Increment(ref sent);
				if (s % 1000 == 0)
					Console.WriteLine($"Sent: {s}");
				await Task.Delay(Random.Shared.Next(200));
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
		finally
		{
			Interlocked.Decrement(ref connected);
			Console.WriteLine($"DC Connected: {connected}");
		}
	}));


	await Task.Delay(10);
}

await Task.WhenAll(tasks);