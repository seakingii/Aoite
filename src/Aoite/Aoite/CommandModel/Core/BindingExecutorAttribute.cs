using System;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型具有指定执行器类型的特性。
    /// <para>备注：若执行命令是一个泛型，执行器类型必须也是一个泛型。</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BindingExecutorAttribute : Attribute
    {
        /// <summary>
        /// 获取执行器类型。
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 初始化一个 <see cref="BindingExecutorAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="type">执行器类型。</param>
        public BindingExecutorAttribute(Type type)
        {
            if(type == null) throw new ArgumentNullException(nameof(type));
            this.Type = type;
        }
    }
}
