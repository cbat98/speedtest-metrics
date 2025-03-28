using Prometheus;

var builder = WebApplication.CreateBuilder(args);

Metrics.SuppressDefaultMetrics();

var metricsService = new MetricsService();
builder.Services.AddSingleton(metricsService);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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

app.Run();
