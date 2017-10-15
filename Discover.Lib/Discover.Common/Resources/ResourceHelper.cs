using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Discover.Resources
{
    public static class ResourceHelper
    {
        public static string ReadEmbeddedResourceAsString(this Assembly assembly, string resourceName)
        {
            using (var reader = new StreamReader(assembly.GetManifestResourceStream(string.Concat(assembly.GetName().Name, ".", resourceName))))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
