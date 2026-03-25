using System;

namespace MegaCrit.Sts2.Core.Modding
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ModInitializerAttribute : Attribute
    {
        public ModInitializerAttribute(string methodName)
        {
        }
    }
}

namespace MegaCrit.Sts2.Core.Runs
{
    public class RunManager
    {
    }
}
