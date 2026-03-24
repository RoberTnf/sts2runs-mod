using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Runs;

#nullable enable

namespace STS2RunsMod
{
    [ModInitializer("Init")]
    public static class Plugin
    {
        [DllImport("libdl.so.2")]
        private static extern IntPtr dlopen(string path, int flags);

        private static Harmony? _harmony;
        internal static ModConfig Config = null!;

        public static void Init()
        {
            try
            {
                Config = ModConfig.Load();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    try
                    {
                        const int RTLD_NOW = 0x2;
                        const int RTLD_GLOBAL = 0x100;
                        dlopen("libgcc_s.so.1", RTLD_NOW | RTLD_GLOBAL);
                        Godot.GD.Print("[STS2RunsMod] Loaded libgcc_s for Harmony compatibility");
                    }
                    catch (Exception ex)
                    {
                        Godot.GD.Print($"[STS2RunsMod] Failed to load libgcc_s: {ex.Message}");
                    }
                }
                _harmony = new Harmony("com.sts2runsmod");

                // Manual postfix on RunManager.OnEnded(bool win)
                var target = AccessTools.Method(typeof(RunManager), "OnEnded");
                var postfix = typeof(RunCompletePatch).GetMethod(nameof(RunCompletePatch.OnRunEnded), BindingFlags.Public | BindingFlags.Static);
                _harmony.Patch(target, postfix: new HarmonyMethod(postfix));

                if (Config.Enabled)
                {
                    Task.Run(async () => await SteamAuth.Authenticate(Config));
                }

                Godot.GD.Print("[STS2RunsMod] Initialized");
            }
            catch (Exception ex)
            {
                Godot.GD.Print($"[STS2RunsMod] Init failed: {ex.Message}");
            }
        }
    }
}
