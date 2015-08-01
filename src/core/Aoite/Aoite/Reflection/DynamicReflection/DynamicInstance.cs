using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Aoite.Reflection
{
    /// <summary>
    /// �ṩ����ָ������ʱ�Ķ�̬��Ϊ��
    /// </summary>
    public sealed class DynamicInstance : DynamicObject
    {
        private readonly object _Target;
        /// <summary>
        /// �ṩһ�������������ʵ������ʼ��һ�� <see cref="Aoite.Reflection.DynamicInstance"/> �����ʵ����
        /// </summary>
        /// <param name="target">һ���������͵�ʵ����</param>
        public DynamicInstance(object target)
        {
            this._Target = target;
        }

        /// <summary>
        /// Sets the member on the target to the given value. Returns true if the value was
        /// actually written to the underlying member.
        /// </summary>     
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return _Target.TrySetValue(binder.Name, value);
        }

        /// <summary>
        /// Gets the member on the target and assigns it to the value parameter. Returns
        /// true if a value other than null was found and false otherwise.
        /// </summary>      
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _Target.TryGetValue(binder.Name);
            return result != null;
        }

        /// <summary>
        /// Invokes the method specified and assigns the value to the value parameter. Returns
        /// true if a method to invoke was found and false otherwise.
        /// </summary>     
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var bindingFlags = Flags.InstanceAnyVisibility | Flags.IgnoreParameterModifiers;
            var method = _Target.GetType().Method(binder.Name, args.ToTypeArray(), bindingFlags);
            result = method == null ? null : method.Call(_Target, args);
            return method != null;
        }

        /// <summary>
        /// Gets all member names from the underlying instance.
        /// </summary>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _Target.GetType().Members().Select(m => m.Name);
        }
    }
}