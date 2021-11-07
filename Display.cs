using System.Diagnostics;

internal class Display {
    private int progressInitialCursorLeft;
    private int progressInitialCursorTop;
    private Stopwatch stopwatch = new Stopwatch();

    public string Version { get; init; } = string.Empty;
    public int ProgressLength { get; init; } = 40;

    public void Info() {
        Console.WriteLine($"Running Newxim manager V{Version}...");
    }

    public void Start(Configuration configuration) {
        Console.WriteLine($"Threads:    {configuration.WorkersPoolSize}");
        Console.WriteLine($"Executalbe: {configuration.ExecutablePath}");
        Console.WriteLine($"Config:     {configuration.ConfigPath}");
        Console.WriteLine($"Argument:   -{configuration.ValueArgument} {configuration.ValuePayload}");
        Console.WriteLine($"Values:     [{configuration.ValueStart}, {configuration.ValueStep}, {configuration.ValueStop}]");
        Console.WriteLine($"Output:     {configuration.OutputPath}");
        Console.Write($"Progress:   ");

        progressInitialCursorLeft = Console.CursorLeft;
        progressInitialCursorTop = Console.CursorTop;
        UpdateProgress(0);
        stopwatch.Start();
    }

    public void Aggregate() {
        Console.WriteLine("Aggregating metrics...");
    }

    public void Export() {
        Console.WriteLine("Exporting metrics...");
    }

    public void ConfigurationFail(string message) {
        Console.Error.WriteLine("Process terminated due to configuration error:");
        Console.Error.WriteLine(message);
    }

    public void RuntimeFail(string message) {
        Console.Error.WriteLine("Process terminated due to runtime error:");
        Console.Error.WriteLine(message);
    }

    public void UpdateProgress(double progress) {
        Console.CursorLeft = progressInitialCursorLeft;
        Console.CursorTop = progressInitialCursorTop;

        Console.Write('[');
        string percentage = $"{progress * 100:F2}%";
        int length = ProgressLength - 2;
        int percentageStart = length / 2 - percentage.Length / 2;
        for (int i = 0; i < length; i++) {
            if (i == percentageStart) {
                Console.Write(percentage);
                i += percentage.Length;
            }

            if (i / (length - 1.0) <= progress) {
                Console.Write('=');
            } else if ((i - 1) / (length - 1.0) < progress) {
                Console.Write('>');
            } else {
                Console.Write('-');
            }
        }
        Console.WriteLine(']');
    }

    public void Finish() {
        stopwatch.Stop();
        Console.WriteLine($"Done in {stopwatch.ElapsedMilliseconds / 1000.0:F2}s");
    }
}