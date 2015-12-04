using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Dbx
{
    public interface IEngine
    {
        IEngineProvider Provider { get; }
    }
}
