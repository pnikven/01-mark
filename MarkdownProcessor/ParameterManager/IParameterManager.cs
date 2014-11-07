using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownProcessor
{
    interface IParameterManager
    {
        string GetFirstParameter(string[] args);
    }
}
