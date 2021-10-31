
internal class WorkersManager {
    private List<WorkerInstance> instances = new ();
    private List<Task> activeInstances = new ();
    private int instancesFinished;
    private int instancesTotal;

    public int WorkersPoolSize { get; init; }
    public string ExecutablePath { get; init; } = string.Empty;
    public string? ConfigPath { get; init; }

    public double ValueStart { get; init; }  
    public double ValueStep { get; init; }
    public double ValueStop { get; init; } 

    public string ValueArgument { get; init; } = string.Empty;
    public string ValuePayload { get; init; } = string.Empty;

    public event EventHandler<ProgressEventArgs>? ProgressChanged;

    private bool SpawnInstance() {
        var id = instances.Count;

        double value = ValueStart + id * ValueStep;
        if (value > ValueStop) {
            return false;
        }

        var arguments = new Dictionary<string, string> {
            { $"-{ValueArgument}", ValuePayload.Replace("%", value.ToString()) }
        };

        var instance = new WorkerInstance {
            SequenceId = id,
            ExecutablePath = ExecutablePath,
            ConfigPath = ConfigPath,
            Arguments = arguments,
        };

        instances.Add(instance);
        activeInstances.Add(instance.Run());

        return true;
    }

    public Task Run() {
        return Task.Run(() => {
            instancesTotal = (int)Math.Ceiling((ValueStop - ValueStart) / ValueStep);

            while (instances.Count < instancesTotal) {
                if (activeInstances.Count < WorkersPoolSize) {
                    SpawnInstance();
                } else {
                    var index = Task.WaitAny(activeInstances.ToArray());
                    activeInstances.RemoveAt(index);

                    instancesFinished++;
                    ProgressChanged?.Invoke(this, new ProgressEventArgs((double)instancesFinished / instancesTotal));
                }
            }

            Task.WaitAll(activeInstances.ToArray());

            instancesFinished += activeInstances.Count;
            ProgressChanged?.Invoke(this, new ProgressEventArgs((double)instancesFinished / instancesTotal));
        });
    }
}