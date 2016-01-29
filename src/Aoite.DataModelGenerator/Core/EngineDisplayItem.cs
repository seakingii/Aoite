using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.DataModelGenerator
{
    public class EngineDisplayItem
    {
        public string Provider { get; set; }
        public string Name { get; set; }
        public string ConnectionStringTemplate { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
