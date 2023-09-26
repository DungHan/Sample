using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Caching
{
    public class KeyGenerator
    {
        public static string GetNameSpace(object target)
        {
            #region Contracts

            if (target == null) throw new ArgumentException($"{nameof(target)} is null");

            #endregion

            // Get name
            var nameSpace = target.GetType().Namespace;

            // Return
            return nameSpace;
        }

        // 使用 namespace + className + MethodName + 參數 產生 Key
        public static string GetKey(object target, [CallerMemberName] string memberName = "")
        {
            #region Contracts

            if (target == null) throw new ArgumentException($"{nameof(target)} is null");

            #endregion

            // Get name
            var nameSpace = target.GetType().Namespace;
            var className = target.GetType().Name;

            // Return
            return $"{nameSpace}:{className}:{memberName}";
        }

        public static string GetKey(object target, string className, string memberName)
        {
            #region Contracts

            if (target == null) throw new ArgumentException($"{nameof(target)} is null");
            if (string.IsNullOrEmpty(className) == true) throw new ArgumentException($"{nameof(className)} is null");
            if (string.IsNullOrEmpty(memberName) == true) throw new ArgumentException($"{nameof(memberName)} is null");

            #endregion

            // Get name
            var nameSpace = target.GetType().Namespace;

            // Return
            return $"{nameSpace}:{className}:{memberName}";
        }

        public static string GetKey(string nameSpace, string className, string memberName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(nameSpace) == true) throw new ArgumentException($"{nameof(nameSpace)} is null");
            if (string.IsNullOrEmpty(className) == true) throw new ArgumentException($"{nameof(className)} is null");
            if (string.IsNullOrEmpty(memberName) == true) throw new ArgumentException($"{nameof(memberName)} is null");

            #endregion

            // Return
            return $"{nameSpace}:{className}:{memberName}";
        }

        public static string Join(params object[] values) => $":{string.Join(".", values.Where(v => v != null).ToArray())}";
    }
}
