using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WebSocketController : ControllerBase
{
	private readonly HttpClient _client;

	public WebSocketController(HttpClient client)
	{
		_client = client;
	}

	public static int ConnectedSocketCount;
	public static int MessagesFromConnectedSockets;

	public async Task Get()
	{
		if (HttpContext.WebSockets.IsWebSocketRequest)
		{
			using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
			int messages = 0;
			try
			{
				Interlocked.Increment(ref ConnectedSocketCount);

				var mem = new byte[1024];
				while (webSocket.State == System.Net.WebSockets.WebSocketState.Open)
				{
					var res = await webSocket.ReceiveAsync(mem, CancellationToken.None);
					if (res.EndOfMessage)
					{
						try
						{
							await _client.GetAsync("http://localhost:5006/WeatherForecast");
							messages++;
							Interlocked.Increment(ref MessagesFromConnectedSockets);
						}
						catch
						{
							//Fails sometimes because we're spamming the server
						}
					}
				}
			}
			finally
			{
				Interlocked.Decrement(ref ConnectedSocketCount);
				Interlocked.Add(ref MessagesFromConnectedSockets, -messages);
			}
		}
		else
		{
			HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
		}
	}
}
