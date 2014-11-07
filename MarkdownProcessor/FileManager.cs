using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownProcessor
{
    class FileManager: IFileManager
    {
        public string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteFile(string filename, string content)
        {
            File.WriteAllText(FormResultFilename(filename), content);
        }

        private static string FormResultFilename(string filename)
        {
            IAssemblyManager assemblyManager = new AssemblyManager();
            var programDirecory = assemblyManager.GetExecutingAssemblyDirectory();
            return Path.Combine(programDirecory, filename);
        }
    }
}
