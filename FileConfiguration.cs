using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NewximManager;



internal class FileConfiguration {
    private class Node {
        public List<Configuration> Configurations { get; set; }
    }

    public List<Configuration> Configurations { get; private set; }

    public FileConfiguration(string[] args) {
        var file = "config.yml";
        int cfg = Array.IndexOf(args, "-config");
        if (cfg + 1 < args.Length) {
            file = args[cfg + 1];
        }

        if (!File.Exists(file)) {
            throw new FileNotFoundException("Configuration file not found.");
        }

        var yml = File.ReadAllText(file);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        Configurations = deserializer.Deserialize<Node>(yml).Configurations;
        foreach (var configuration in Configurations) {
            configuration.Arguments = configuration.Arguments.ToDictionary(item => "-" + item.Key, item => item.Value);
            configuration.Check();
        }
    }
}
