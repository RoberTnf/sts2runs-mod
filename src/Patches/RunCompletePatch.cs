using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Runs;

#nullable enable

namespace STS2RunsMod
{
    public static class RunCompletePatch
    {
        public static void OnRunEnded(RunManager __instance, bool __0)
        {
            if (!Plugin.Config.Enabled)
                return;

            bool win = __0;

            Task.Run(async () =>
            {
                try
                {
                    var modCheck = ModDetector.Check();
                    if (!modCheck.CanUpload)
                    {
                        Godot.GD.Print($"[STS2RunsMod] Upload skipped: gameplay-affecting mods detected ({string.Join(", ", modCheck.BlockingModNames)})");
                        return;
                    }

                    await Task.Delay(500);
                    var result = RunExporter.ReadLatestRunFile();
                    await Uploader.Upload(result.Json, result.Profile);

                    Godot.GD.Print($"[STS2RunsMod] Run uploaded (win={win})");
                }
                catch (Exception ex)
                {
                    Godot.GD.Print($"[STS2RunsMod] Upload failed: {ex.Message}");
                }
            });
        }
    }
}
