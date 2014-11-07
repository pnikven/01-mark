using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownProcessor
{
    interface IFileManager
    {
        string ReadFile(string path);
        void WriteFile(string filename, string content);
    }
}
