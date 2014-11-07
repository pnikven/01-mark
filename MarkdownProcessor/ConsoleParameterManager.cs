using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownProcessor
{
    class ConsoleParameterManager: IParameterManager 
    {
        public string GetFirstParameter(string[] consoleArguments)
        {
            if (consoleArguments.Length != 0) return consoleArguments[0];
            throw new Exception("File name must be provided");
        }
    }
}
