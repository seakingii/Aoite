using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Serialization.Json
{
    /// <summary>
    /// 提供用于实现自定义类型解析器的抽象基类。
    /// </summary>
    public abstract class JTypeResolver
    {

        /// <summary>
        /// 初始化 <see cref="JTypeResolver"/> 类的新实例。
        /// </summary>
        protected JTypeResolver() { }

        /// <summary>
        /// 当在派生类中重写时，返回与指定类型名称相关联的 <see cref="Type"/> 对象。
        /// </summary>
        /// <param name="id">托管类型的名称。</param>
        /// <returns>与指定类型名称相关联的 <see cref="Type"/> 对象。</returns>
        public abstract Type ResolveType(string id);
        /// <summary>
        /// 当在派生类中重写时，返回指定的 <see cref="Type"/> 对象的类型名称。
        /// </summary>
        /// <param name="type">要解析的托管类型。</param>
        /// <returns>指定托管类型的名称。</returns>
        public abstract string ResolveTypeId(Type type);
    }
}
