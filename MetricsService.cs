using Prometheus;

public class MetricsService
{
    private readonly Counter _sampleCounter;
    private readonly Gauge _sampleGauge;
    private readonly Histogram _sampleHistogram;

    private readonly Gauge _sampleLabelGauge;

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

        _sampleLabelGauge = Metrics.CreateGauge(
            "sample_data_table",
            "Sample data to populate a table",
            new GaugeConfiguration
            {
                LabelNames = new[] { "labelOne", "labelTwo", "labelThree" }
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

        _sampleLabelGauge.WithLabels("123", "456", "789").Set(1);
    }
}

