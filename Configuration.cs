namespace NewximManager;

internal class Configuration {
    private const string defaultExecutablePath = "Newxim.exe";
    private const string defaultOutputPath = "result.csv";

    public int WorkersPoolSize { get; set; } = 8;
    public string ExecutablePath { get; set; } = defaultExecutablePath;
    public string ConfigPath { get; set; }

    public double? ValueStart { get; set; }
    public double? ValueStep { get; set; }
    public double? ValueStop { get; set; }

    public ExporterType Exporter { get; set; } = ExporterType.CSV;

    public string OutputPath { get; set; } = defaultOutputPath;

    public IDictionary<string, string> Arguments { get; set; } = new Dictionary<string, string>();

    public Configuration() {
    }

    public Configuration(string[] args) {
        for (int i = 0; i < args.Length; i++) {
            var arg = args[i];

            switch (arg) {
            case "-t":
                ParseWorkersPoolSize(args, ref i);
                break;
            case "-x":
                ParseExecutablePath(args, ref i);
                break;
            case "-c":
                ParseConfigPath(args, ref i);
                break;
            case "-v":
                ParseValue(args, ref i);
                break;
            case "-e":
                ParseExporter(args, ref i);
                break;
            case "-o":
                ParseOutput(args, ref i);
                break;
            default:
                if (arg.StartsWith("--")) {
                    ParseArgument(args, ref i);
                } else {
                    FailUnknownOption(arg);
                }
                break;
            }
        }
    }

    private void ParseWorkersPoolSize(string[] args, ref int i) {
        if (i + 1 >= args.Length || !int.TryParse(args[++i], out int val) || val <= 0) {
            throw new Exception("Option -t <threads> requires a positive integer value");
        }
        WorkersPoolSize = val;
    }

    private void ParseExecutablePath(string[] args, ref int i) {
        if (i + 1 >= args.Length) {
            throw new Exception("Option -x <executalbe> requires a valid executable path");
        }
        ExecutablePath = args[++i];
    }

    private void ParseConfigPath(string[] args, ref int i) {
        if (i + 1 >= args.Length) {
            throw new Exception("Option -c <config> requires a valid config path");
        }
        ConfigPath = args[++i];
    }

    private void ParseValue(string[] args, ref int i) {
        if (i + 5 >= args.Length) {
            throw new Exception("Option -v <start> <step> <stop> requires five arguments");
        }
        if (!double.TryParse(args[++i], out double start)) {
            throw new Exception("Option -v <start> requires a real value");
        }
        ValueStart = start;
        if (!double.TryParse(args[++i], out double step) || step <= 0) {
            throw new Exception("Option -v <step> requires a positive real value");
        }
        ValueStep = step;
        if (!double.TryParse(args[++i], out double stop) || stop <= start) {
            throw new Exception("Option -v <stop> requires a real value that is grater than <start>");
        }
        ValueStop = stop;
    }

    private void ParseExporter(string[] args, ref int i) {
        if (i + 1 >= args.Length) {
            throw new Exception("Option -e <exporter> requires a valid config path");
        }
        if (!Enum.TryParse<ExporterType>(args[++i], true, out var exporter)) {
            throw new Exception($"Invalid exporter. Allowed types: {string.Join(", ", Enum.GetNames<ExporterType>())}");
        }
        Exporter = exporter;
    }

    private void ParseOutput(string[] args, ref int i) {
        if (i + 1 >= args.Length) {
            throw new Exception("Option -o <path> requires a file name");
        }
        OutputPath = args[++i];
    }

    private void ParseArgument(string[] args, ref int i) {
        if (i + 1 >= args.Length) {
            throw new Exception($"Passthrough argument {args[i]} <value> requires parameter");
        }
        Arguments.Add(args[i].Substring(1), args[++i]);
    }

    private void FailUnknownOption(string arg) {
        throw new Exception($"Unknown option [{arg}]");
    }

    public void Check() {
        if (ValueStart is null) {
            throw new Exception("Option -v is required");
        }
        if (!File.Exists(ExecutablePath)) {
            throw new Exception($"Executable file [{ExecutablePath}] does not exists");
        }
        if (ConfigPath is not null && !File.Exists(ConfigPath)) {
            throw new Exception($"Configuration file [{ConfigPath}] does not exists");
        }
    }

    public void CopyTo(Configuration configuration) {
        if (WorkersPoolSize != 8) {
            configuration.WorkersPoolSize = WorkersPoolSize;
        }
        if (ExecutablePath != defaultExecutablePath) {
            configuration.ExecutablePath = ExecutablePath;
        }
        if (ConfigPath is not null) {
            configuration.ConfigPath = ConfigPath;
        }
        if (ValueStart is not null) {
            configuration.ValueStart = ValueStart;
        }
        if (ValueStep is not null) {
            configuration.ValueStep = ValueStep;
        }
        if (ValueStop is not null) {
            configuration.ValueStop = ValueStop;
        }
        if (Exporter != ExporterType.CSV) {
            configuration.Exporter = Exporter;
        }
        if (OutputPath != defaultOutputPath) {
            configuration.OutputPath = OutputPath;
        }
        foreach (var argument in Arguments) {
            configuration.Arguments.Add(argument);
        }
    }
}