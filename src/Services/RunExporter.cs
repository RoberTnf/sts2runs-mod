using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#nullable enable

namespace STS2RunsMod
{
    public record RunResult(string Json, int Profile);

    public static class RunExporter
    {
        private static readonly string SaveBase =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "SlayTheSpire2");

        public static RunResult ReadLatestRunFile()
        {
            var profileDirs = Directory.GetDirectories(Path.Combine(SaveBase, "steam"))
                .SelectMany(userDir => new[] { userDir, Path.Combine(userDir, "modded") })
                .Where(Directory.Exists)
                .SelectMany(dir => Directory.GetDirectories(dir, "profile*"));

            var newest = profileDirs
                .Select(profileDir => Path.Combine(profileDir, "saves", "history"))
                .Where(Directory.Exists)
                .SelectMany(dir => new DirectoryInfo(dir).GetFiles("*.run"))
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .FirstOrDefault();

            if (newest == null)
                throw new FileNotFoundException("No .run files found in any history directory");

            Godot.GD.Print($"[STS2RunsMod] Reading run file: {newest.FullName}");

            var profileDir = newest.Directory!.Parent!.Parent!;
            var match = Regex.Match(profileDir.Name, @"profile(\d+)");
            int profile = match.Success ? int.Parse(match.Groups[1].Value) : 0;

            return new RunResult(File.ReadAllText(newest.FullName), profile);
        }
    }
}
