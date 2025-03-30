using Prometheus;

var builder = WebApplication.CreateBuilder(args);

Metrics.SuppressDefaultMetrics();

builder.Services.AddSingleton<MetricsService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Use(async (context, next) =>
{
    Console.WriteLine($"[{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}] - INFO - {context.Connection.RemoteIpAddress}, {context.Request.Method}, {context.Request.Path}");
    await next();
});

app.MapGet("/", () => "Hello, world"!);
app.MapGet("/metrics", async (MetricsService metricsService) =>
{
    metricsService.Populate();

    var ms = new MemoryStream();
    await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(ms).ConfigureAwait(false);
    ms.Position = 0;

    var sr = new StreamReader(ms);
    return await sr.ReadToEndAsync().ConfigureAwait(false);
});
app.MapGet("/speed", () =>
{
    SpeedTest.Run();

    return new
    {
        ping = SpeedTest.LatestPing,
        downloadMbps = (float)SpeedTest.LatestDownloadBandwidth * 8 / 1000000,
        uploadMbps = (float)SpeedTest.LatestUploadBandwidth * 8 / 1000000,
        timestamp = SpeedTest.LatestTimestamp
    };
});

app.Run();
