using Server;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry(o =>
{
	o.Dsn = "Populate me";

	o.SetBeforeSend(_ => { return null; }); //Don't send anything to sentry
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddHostedService<MemoryMonitorService>();

//Stop HttpClient spam in the debug log
builder.Logging.ClearProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseWebSockets();
app.MapControllers();

app.Run();
