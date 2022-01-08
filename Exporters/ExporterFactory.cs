namespace NewximManager.Exporters; 



internal static class ExporterFactory {
    public static IExporter Make(ExporterType type) {
        return type switch {
            ExporterType.CSV => new CSVExporter(),
            _ => throw new NotImplementedException(),
        };
    }
}