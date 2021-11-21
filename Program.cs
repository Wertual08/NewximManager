// -----DEBUG----- //
args = new string[] { 
    "-x",
    "../../../../bin/x64/Release/Newxim.exe",
    "-c",
    "../../../../Newxim/config.yml",
    "-v",
    "0",
    "0.01",
    "1",
    "-e",
    "csv",

    "--packet_injection_rate",
    "%",
};
// ---END-DEBUG--- //


var display = new Display {
    Version = "0.0.0.0",
};
display.Info();

Configuration configuration;
try {
    configuration = new Configuration(args);
} catch (Exception e) {
    display.ConfigurationFail(e.Message);
    return 1;
}

try {
    var manager = new WorkersManager {
        WorkersPoolSize = configuration.WorkersPoolSize,
        ExecutablePath = configuration.ExecutablePath,
        ConfigPath = configuration.ConfigPath,
        ValueStart = configuration.ValueStart.Value,
        ValueStep = configuration.ValueStep.Value,
        ValueStop = configuration.ValueStop.Value,
        Arguments = configuration.Arguments,
    };
    manager.ProgressChanged += (object? sender, ProgressEventArgs e) => {
        lock (display) {
            display.UpdateProgress(e.Progress);
        }
    };

    display.Start(configuration);
    await manager.Run();

    display.Aggregate();
    var metrics = manager.AggregateMetrics();

    display.Export();
    var exporter = ExporterFactory.Make(configuration.Exporter);
    using var file = File.OpenWrite(configuration.OutputPath);
    exporter.Export(metrics, file);
} catch (Exception e) {
    display.RuntimeFail(e.Message);
    return 2;
}

display.Finish();
return 0;