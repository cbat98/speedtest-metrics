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

public class MetricsService
{
    private readonly Counter _sampleCounter;
    private readonly Gauge _sampleGauge;
    private readonly Histogram _sampleHistogram;

    public MetricsService()
    {
        _sampleCounter = Metrics.CreateCounter(
            "sample_requests_total",
            "Total number of sample requests",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

        _sampleGauge = Metrics.CreateGauge(
            "sample_active_users",
            "Number of active users");

        _sampleHistogram = Metrics.CreateHistogram(
            "sample_duration_seconds",
            "Sample duration in seconds",
            new HistogramConfiguration
            {
                Buckets = new[] { 0.1, 0.5, 1.0, 2.0 }
            });
    }

    public void Populate()
    {
        _sampleCounter.WithLabels("success").Inc(42);
        _sampleCounter.WithLabels("failed").Inc(7);

        _sampleGauge.Set(15);

        _sampleHistogram.Observe(0.3);
        _sampleHistogram.Observe(0.8);
        _sampleHistogram.Observe(1.5);
    }
}

static class SpeedTest
{
    public static string Run()
    {
        var cliOutput = "";

        using (var speedtestCli = new System.Diagnostics.Process())
        {
            speedtestCli.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"speedtest.exe");
            speedtestCli.StartInfo.Arguments = "--format=json --progress=no --accept-license --accept-gdpr";
            speedtestCli.StartInfo.RedirectStandardOutput = true;

            speedtestCli.Start();
            speedtestCli.WaitForExit();

            cliOutput = speedtestCli.StandardOutput.ReadToEnd();

            speedtestCli.Close();
        }

        return cliOutput;
    }
}
