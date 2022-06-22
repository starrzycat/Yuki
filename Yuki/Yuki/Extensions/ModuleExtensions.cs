using Qmmands;

namespace Yuki.Extensions
{
    public static class ModuleExtensions
    {
        public static bool IsSubmodule(this Module module)
        {
            return module.Parent != null;
        }
    }
}
