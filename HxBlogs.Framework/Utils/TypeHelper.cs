namespace HxBlogs.Framework.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Compilation;

    internal static class TypeHelper
    {
        internal static IEnumerable<MethodInfo> GetStaticMethods(this Type type)
        {
            return type.GetRuntimeMethods().Where(m => m.IsStatic);
        }
        private static List<Assembly> _allAssembly = null;
        /// <summary>
        /// ��ȡ���г���
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> GetAllAssembly()
        {
            if (_allAssembly == null || _allAssembly.Count == 0)
            {
                _allAssembly = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            }
            return _allAssembly;
        }
    }
}