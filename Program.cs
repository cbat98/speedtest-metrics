using Prometheus;

var builder = WebApplication.CreateBuilder(args);

Metrics.SuppressDefaultMetrics();

var metricsService = new MetricsService();
builder.Services.AddSingleton(metricsService);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/metrics")
    {
        var metricsService = app.Services.GetService<MetricsService>();

        if (metricsService is null)
            return;

        metricsService.Populate();
        await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(context.Response.Body, context.RequestAborted);

        return;
    }
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello, world"!);
app.MapGet("/speedtest", () => SpeedTest.Run());

app.Run();
