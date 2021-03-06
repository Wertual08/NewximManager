using System.Diagnostics;

namespace NewximManager;


internal class Display {
    private int progressInitialCursorLeft;
    private int progressInitialCursorTop;
    private Stopwatch stopwatch = new Stopwatch();

    public string Version { get; init; } = string.Empty;
    public int ProgressLength { get; init; } = 40;

    public void Info() {
        Console.WriteLine($"Newxim manager V{Version}");
        Console.WriteLine();
    }

    public void Start(int index, Configuration configuration) {
        Console.WriteLine($"[{index}] Running simulation...");
        Console.WriteLine($"Threads:    {configuration.WorkersPoolSize}");
        Console.WriteLine($"Executalbe: {configuration.ExecutablePath}");
        Console.WriteLine($"Config:     {configuration.ConfigPath}");
        Console.WriteLine($"Values:     [{configuration.ValueStart}, {configuration.ValueStep}, {configuration.ValueStop}]");
        Console.WriteLine($"Output:     {configuration.OutputPath}");
        foreach (var argument in configuration.Arguments) {
            Console.WriteLine($"Argument:   {argument.Key} {argument.Value}");
        }
        Console.Write($"Progress:   ");

        (progressInitialCursorLeft, progressInitialCursorTop) = Console.GetCursorPosition();
        UpdateProgress(0);
        stopwatch.Start();
    }

    public void Aggregate() {
        Console.WriteLine("Aggregating metrics...");
    }

    public void Export() {
        Console.WriteLine("Exporting metrics...");
    }

    public void Stop() {
        Console.WriteLine();
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
        Console.SetCursorPosition(progressInitialCursorLeft, progressInitialCursorTop);
        
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
        Console.Write(']');
    }

    public void Finish() {
        stopwatch.Stop();
        Console.WriteLine();
        Console.WriteLine($"Done in {stopwatch.ElapsedMilliseconds / 1000.0:F2}s");
    }
}