internal class MetricsAggregation {
    public List<string> Headers { get; private set; } = new List<string>();
    public List<List<double?>> Values { get; private set; } = new List<List<double?>>();

    public void Add(IDictionary<string, double>? entries) {
        ArgumentNullException.ThrowIfNull(nameof(entries));

        var row = new List<double?>(Headers.Count);
        foreach (var entry in entries) {
            var index = Headers.IndexOf(entry.Key);
            if (index < 0) {
                index = Headers.Count;
                Headers.Add(entry.Key);
            }
            while (row.Count <= index) {
                row.Add(null);
            }
            row[index] = entry.Value;
        }
        Values.Add(row);
    }

    public int ColumnCount { get => Headers.Count; }
    public int RowCount { get => Values.Count; }

    public string this[int c] {
        get => Headers[c]; 
    }

    public double? this[int c, int r] {
        get {
            if (r < 0 || r >= Values.Count) {
                return null;
            }
            if (c < 0 || c >= Values[r].Count) {
                return null;
            }
            return Values[r][c];
        }
    }
}
