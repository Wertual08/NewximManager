using System.Text;

internal class CSVExporter : IExporter {
    private string Escape(string value) {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    public void Export(MetricsAggregation metrics, Stream stream) {
        using var writer = new StreamWriter(stream, leaveOpen: true);

        writer.WriteLine(string.Join(',', from header in metrics.Headers select Escape(header)));

        StringBuilder builder = new StringBuilder();
        for (int y = 0; y < metrics.RowCount; y++) {
            builder.Clear();
            for (int x = 0; x < metrics.ColumnCount; x++) {
                double? value = metrics[x, y];
                if (value.HasValue) {
                    builder.Append(Escape(value.Value.ToString()));
                }
                if (x < metrics.ColumnCount - 1) {
                    builder.Append(','); 
                }
            }
            writer.WriteLine(builder.ToString());
        }
    }
}