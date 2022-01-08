namespace NewximManager.Exporters; 



internal interface IExporter {
    void Export(MetricsAggregation metrics, Stream stream);
}