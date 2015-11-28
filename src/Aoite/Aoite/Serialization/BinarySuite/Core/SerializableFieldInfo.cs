using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Aoite.Reflection;

namespace Aoite.Serialization
{
    internal class SerializableFieldInfo
    {
        public readonly FieldInfo Field;
        public readonly string Name;
        //public readonly int NameHashCode;
        public readonly DynamicMemberGetter GetValue;
        public readonly DynamicMemberSetter SetValue;

        public SerializableFieldInfo(FieldInfo field, int depth)
        {
            this.Field = field;
            if(depth == 0) this.Name = field.Name;
            else this.Name = depth.ToString() + "#" + field.Name;
            this.GetValue = field.CreateFieldGetter();
            this.SetValue = field.CreateFieldSetter();
            
            //this.Name
            //this.NameHashCode = this.Name.GetHashCode();
        }

    }
}
