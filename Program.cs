// -----DEBUG----- //
args = new string[] { 
    "-x",
    "../../../../bin/x64/Release/Newxim.exe",
    "-c",
    "../../../../Newxim/config.yml",
    "-v",
    "packet_injection_rate",
    "%",
    "0",
    "0.01",
    "1",
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
        ValueArgument = configuration.ValueArgument,
        ValuePayload = configuration.ValuePayload,
        ValueStart = configuration.ValueStart.Value,
        ValueStep = configuration.ValueStep.Value,
        ValueStop = configuration.ValueStop.Value,
    };
    manager.ProgressChanged += (object? sender, ProgressEventArgs e) => {
        lock (display) {
            display.UpdateProgress(e.Progress);
        }
    };

    display.Start(configuration);
    await manager.Run();
} catch (Exception e) {
    display.RuntimeFail(e.Message);
    return 2;
}

display.Finish();
return 0;