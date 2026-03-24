using System.IO;
using System.Reflection;
using System.Text.Json;

namespace STS2RunsMod
{
    public class ModConfig
    {
        public string ServerUrl { get; set; } = "https://sts2runs.com";
        public bool Enabled { get; set; } = true;

        public static ModConfig Load()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(dir, "config.cfg");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<ModConfig>(json) ?? new ModConfig();
            }
            else
            {
                ModConfig config = new ModConfig();
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
                Godot.GD.Print($"[STS2RunsMod] Config created at {path}");
                return config;
            }
        }
    }
}
