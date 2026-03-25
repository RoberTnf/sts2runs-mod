using System;
using System.Reflection;

namespace HarmonyLib
{
    public class Harmony
    {
        public Harmony(string id)
        {
        }

        public MethodInfo Patch(
            MethodBase original,
            HarmonyMethod? prefix = null,
            HarmonyMethod? postfix = null,
            HarmonyMethod? transpiler = null,
            HarmonyMethod? finalizer = null)
        {
            return null!;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class HarmonyMethod : Attribute
    {
        public HarmonyMethod(MethodInfo method)
        {
        }
    }

    public static class AccessTools
    {
        public static MethodInfo? Method(Type type, string name, Type[]? parameters = null, Type[]? generics = null)
        {
            return type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }
    }
}
