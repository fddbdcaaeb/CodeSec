using System.Reflection;
using System.Threading.Tasks;

namespace CodeSec.Core.Extension
{
    public static class MethodInfoExtension
    {
        public static bool IsAsync(this MethodInfo info)
        {
            return info.ReturnType.IsAssignableFrom(typeof(Task))
                || info.ReturnType.IsSubclassOf(typeof(Task));
        }
    }
}
