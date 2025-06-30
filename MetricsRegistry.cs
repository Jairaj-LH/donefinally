using Prometheus;

public static class MetricsRegistry
{
    // Create a counter metric
    public static readonly Counter MyCustomCounter = Metrics.CreateCounter(
        "myapp_custom_counter_total",
        "Counts how many times a custom event happens"
    );
}
