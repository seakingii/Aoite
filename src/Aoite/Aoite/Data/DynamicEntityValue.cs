using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Aoite.Data
{
    class DynamicEntityValue : System.Dynamic.DynamicObject
    {
        internal readonly Dictionary<string, object> NameValues = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
        public DynamicEntityValue(DbDataReader reader)
        {
            for(int i = 0; i < reader.FieldCount; i++)
            {
                this.NameValues.Add(reader.GetName(i), reader.GetValue(i));
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.NameValues.Keys;
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            return this.NameValues.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            this.NameValues[binder.Name] = value;
            return true;
        }
    }
}
