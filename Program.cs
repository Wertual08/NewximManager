using NewximManager;
using NewximManager.Exporters;



var display = new Display {
    Version = "0.0.0.2",
};
display.Info();

List<Configuration> configurations;

int cfg = Array.IndexOf(args, "-config");
try {
    if (cfg >= 0) {
        var fileConfiguration = new FileConfiguration(args);
        configurations = fileConfiguration.Configurations;
    } else {
        var configuration = new Configuration(args);
        configuration.Check();
        configurations = new List<Configuration> {
            configuration
        };
    }
} catch (Exception e) {
    display.ConfigurationFail(e.Message);
    return 1;
}

bool error = false;
for (int i = 0; i < configurations.Count; i++) {
    var configuration = configurations[i];

    try {
        var manager = new WorkersManager {
            WorkersPoolSize = configuration.WorkersPoolSize,
            ExecutablePath = configuration.ExecutablePath,
            ConfigPath = configuration.ConfigPath,
            ValueStart = configuration.ValueStart!.Value,
            ValueStep = configuration.ValueStep!.Value,
            ValueStop = configuration.ValueStop!.Value,
            Arguments = configuration.Arguments,
        };
        manager.ProgressChanged += (object? sender, ProgressEventArgs e) => {
            lock (display) {
                display.UpdateProgress(e.Progress);
            }
        };

        display.Start(i, configuration);
        await manager.Run();

        display.Aggregate();
        var metrics = manager.AggregateMetrics();

        display.Export();
        var exporter = ExporterFactory.Make(configuration.Exporter);
        using var file = File.Create(configuration.OutputPath);
        exporter.Export(metrics, file);

        display.Stop();
    } catch (Exception e) {
        display.RuntimeFail(e.Message);
        error = true;
    }
}

display.Finish();
return error ? 2 : 0;