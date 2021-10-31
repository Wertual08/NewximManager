using System.Diagnostics;
using System.Text;
using System.Text.Json;

internal class WorkerInstance {
    private const string resultPrefix = "% Result: ";

    public int SequenceId { get; init; }
    public string ExecutablePath { get; init; } = string.Empty;
    public string ?ConfigPath { get; init; }
    public IEnumerable<KeyValuePair<string, string>> Arguments { get; init; } = Enumerable.Empty<KeyValuePair<string, string>>();

    public Dictionary<string, double>? Result { get; private set; }

    public Task Run() {
        return Task.Run(() => {
            var args = new StringBuilder();
            if (ConfigPath is not null) {
                args.Append("\"-config\" ");
                args.Append($"\"{ConfigPath}\" ");
            }
            foreach (var arg in Arguments) {
                args.Append($"\"-{arg.Key}\" ");
                args.Append($"\"{arg.Value}\" ");
            }

            using var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = ExecutablePath,
                    Arguments = args.ToString(),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                },

            };
            
            var output = new StringBuilder();
            var error = new StringBuilder();
            process.OutputDataReceived += (sender, e) => output.Append(e.Data);
            process.ErrorDataReceived += (sender, e) => error.Append(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
            string outputStr = output.ToString();
            string errorStr = error.ToString();

            if (process.ExitCode != 0) {
                throw new Exception(errorStr + outputStr);
            }

            int start = outputStr.IndexOf(resultPrefix);
            if (start < 0) {
                throw new Exception("Failed to parse simulation results.");
            }
            start = start + resultPrefix.Length;
            int end = outputStr.LastIndexOf('}') + 1;
            var result = outputStr.Substring(start, end - start);
            Result = JsonSerializer.Deserialize<Dictionary<string, double>>(result);
        });
    }
}