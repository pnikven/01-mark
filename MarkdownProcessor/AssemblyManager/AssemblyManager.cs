using System.IO;
using System.Reflection;

namespace MarkdownProcessor
{
    class AssemblyManager:IAssemblyManager
    {
        public string GetExecutingAssemblyDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
