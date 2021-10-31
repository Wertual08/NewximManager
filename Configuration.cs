
internal class Configuration {
    private const string defaultExecutablePath = "Newxim.exe";

    public int WorkersPoolSize { get; private set; } = 8;
    public string ExecutablePath { get; private set; } = defaultExecutablePath;
    public string? ConfigPath { get; private set; }

    public string? ValueArgument { get; private set; }
    public string? ValuePayload { get; private set; }
    public double? ValueStart { get; private set; }
    public double? ValueStep { get; private set; }
    public double? ValueStop { get; private set; }


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
            default:
                FailUnknownOption(arg);
                break;
            }
        }

        Check();
    }

    private void ParseWorkersPoolSize(string[] args, ref int i) {
        if (i + 1 >= args.Length || !int.TryParse(args[++i], out int val) || val <= 0) {
            throw new Exception("Option -t <threads> requires a positive integer value");
        }
        WorkersPoolSize = val;
    }

    private void ParseExecutablePath(string[] args, ref int i) {
        string path;
        if (i + 1 >= args.Length) {
            throw new Exception("Option -x <executalbe> requires a valid executable path");
        }
        ExecutablePath = args[++i];
    }

    private void ParseConfigPath(string[] args, ref int i) {
        string path;
        if (i + 1 >= args.Length) {
            throw new Exception("Option -c <config> requires a valid config path");
        }
        ConfigPath = args[++i];
    }

    private void ParseValue(string[] args, ref int i) {
        if (i + 5 >= args.Length) {
            throw new Exception("Option -v <option> <value> <start> <step> <stop> requires five arguments");
        }
        ValueArgument = args[++i];
        ValuePayload = args[++i];
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

    private void FailUnknownOption(string arg) {
        throw new Exception($"Unknown option [{arg}]");
    }

    private void Check() {
        if (ValueArgument is null) {
            throw new Exception("Option -v is required");
        }
        if (!File.Exists(ExecutablePath)) {
            throw new Exception($"Executable file [{ExecutablePath}] does not exists");
        }
        if (ConfigPath is not null && !File.Exists(ConfigPath)) {
            throw new Exception($"Configuration file [{ConfigPath}] does not exists");
        }
    }
}