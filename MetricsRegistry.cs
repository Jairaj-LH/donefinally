using Prometheus;

public static class MetricsRegistry
{
    // Create a counter metric
    public static readonly Counter MyCustomCounter = Metrics.CreateCounter(
        "myapp_custom_counter_total",
        "Counts how many times a custom event happens"
    );
    public static readonly Histogram CharacterProcessingDuration = Metrics.CreateHistogram(
    "character_processing_duration_seconds",
    "Time taken to process characters",
    new HistogramConfiguration
    {
        LabelNames = new[] { "action", "subjectId" },
        Buckets = Histogram.ExponentialBuckets(start: 0.001, factor: 2, count: 10)
    }
);

}
