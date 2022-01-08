using NewximManager;
using NewximManager.Exporters;



// -----DEBUG----- //
args = new string[] { 
    //"-x",
    //"../../../../bin/x64/Release/Newxim.exe",
    //"-c",
    //"../../../../Newxim/config.yml",
    //"-v",
    //"0",
    //"0.01",
    //"0.4",
    //"-e",
    //"csv",
    //"-t",
    //"8",

    //"--packet_injection_rate",
    //"%",

    //"--topology",
    //"TORUS",

    ////"--topology_args",
    ////"[64, 5, 6]",

    //"--routing_algorithm",
    //"TABLE_BASED",

    //"--selection_strategy",
    //"RANDOM",

    "-config",
    "../../../config.yml"
};
// ---END-DEBUG--- //


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
foreach (var configuration in configurations) {
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
        manager.ProgressChanged += (object sender, ProgressEventArgs e) => {
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
        using var file = File.Create(configuration.OutputPath);
        exporter.Export(metrics, file);
    } catch (Exception e) {
        display.RuntimeFail(e.Message);
        error = true;
    }
}

display.Finish();
return error ? 2 : 0;