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

	public async Task Get()
	{
		if (HttpContext.WebSockets.IsWebSocketRequest)
		{
			using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

			var mem = new byte[1024];
			while (webSocket.State == System.Net.WebSockets.WebSocketState.Open)
			{
				var res = await webSocket.ReceiveAsync(mem, CancellationToken.None);
				if (res.EndOfMessage)
				{
					try
					{
						await _client.GetAsync("http://localhost:5006/WeatherForecast");
					}
					catch
					{
						//Fails sometimes because we're spamming the server
					}
				}
			}
		}
		else
		{
			HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
		}
	}
}
