using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace STS2RunsMod
{
    public record ModInfo(string Id, string Name, string Version, bool AffectsGameplay);

    public record ModCheckResult(bool CanUpload, List<ModInfo> AllMods, List<string> BlockingModNames);

    public static class ModDetector
    {
        public static ModCheckResult Check()
        {
            try
            {
                string executablePath = Godot.OS.GetExecutablePath();
                string dataDir = Path.GetDirectoryName(executablePath);
                string gameDir = Path.GetDirectoryName(dataDir);
                string modsDir = Path.Combine(gameDir, "mods");

                if (!Directory.Exists(modsDir))
                {
                    Godot.GD.Print("[STS2RunsMod] No mods directory found, upload allowed");
                    return new ModCheckResult(true, new List<ModInfo>(), new List<string>());
                }

                List<ModInfo> allMods = new List<ModInfo>();
                List<string> blockingModNames = new List<string>();

                foreach (string modDir in Directory.GetDirectories(modsDir))
                {
                    string manifestPath = Path.Combine(modDir, "mod_manifest.json");

                    if (!File.Exists(manifestPath))
                        continue;

                    try
                    {
                        string json = File.ReadAllText(manifestPath);
                        using JsonDocument doc = JsonDocument.Parse(json);
                        JsonElement root = doc.RootElement;

                        string id = root.TryGetProperty("id", out JsonElement idEl) ? idEl.GetString() ?? "" : "";
                        string name = root.TryGetProperty("name", out JsonElement nameEl) ? nameEl.GetString() ?? "" : "";
                        string version = root.TryGetProperty("version", out JsonElement versionEl) ? versionEl.GetString() ?? "" : "";
                        bool affectsGameplay = true;

                        if (root.TryGetProperty("affects_gameplay", out JsonElement affectsEl) && affectsEl.ValueKind == JsonValueKind.False)
                            affectsGameplay = false;

                        if (id == "STS2RunsMod")
                            continue;

                        ModInfo mod = new ModInfo(id, name, version, affectsGameplay);
                        allMods.Add(mod);

                        if (affectsGameplay)
                            blockingModNames.Add(name);
                    }
                    catch (Exception ex)
                    {
                        string dirName = Path.GetFileName(modDir);
                        Godot.GD.Print($"[STS2RunsMod] Failed to parse manifest in {dirName}: {ex.Message}, treating as blocking");
                        blockingModNames.Add(dirName);
                    }
                }

                bool canUpload = blockingModNames.Count == 0;
                return new ModCheckResult(canUpload, allMods, blockingModNames);
            }
            catch (Exception ex)
            {
                Godot.GD.Print($"[STS2RunsMod] ModDetector.Check failed: {ex.Message}, defaulting to safe state");
                return new ModCheckResult(false, new List<ModInfo>(), new List<string>());
            }
        }
    }
}
