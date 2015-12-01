using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Dbx
{
    /// <summary>
    /// 表示一个数据源交互的执行命令参数集合。
    /// </summary>
    [Serializable]
    public class ExecuteParameterCollection : ICollection<ExecuteParameter>
    {
        private readonly static Type ExecuteParameterType = typeof(ExecuteParameter);
        private readonly Dictionary<string, ExecuteParameter> _innerDict;

        /// <summary>
        /// 初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        public ExecuteParameterCollection() : this(4) { }

        /// <summary>
        /// 指定初始容量初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="capacity">集合可包含的初始元素数。</param>
        public ExecuteParameterCollection(int capacity)
        {
            this._innerDict = new Dictionary<string, ExecuteParameter>(capacity, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public ExecuteParameterCollection(object objectInstance) : this()
        {
            this.AddObject(objectInstance);
        }

        /// <summary>
        /// 指定参数集合初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="keysAndValues">应当是 <see cref="String"/> / <see cref="Object"/> 的字典集合。</param>
        public ExecuteParameterCollection(params object[] keysAndValues) : this(keysAndValues == null ? 0 : keysAndValues.Length / 2)
        {
            if(keysAndValues == null) return;

            if(keysAndValues.Length % 2 != 0) throw new ArgumentException("参数长度无效！长度必须为 2 的倍数。", "keysAndValues");

            for(int i = 0; i < keysAndValues.Length;)
            {
                this.Add(keysAndValues[i++].ToString(), keysAndValues[i++]);
            }
        }

        /// <summary>
        /// 指定参数数组初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="source">参数数组。</param>
        public ExecuteParameterCollection(params ExecuteParameter[] source) : this((ICollection<ExecuteParameter>)source) { }

        /// <summary>
        /// 指定原参数集合初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="source">原参数集合。</param>
        public ExecuteParameterCollection(ICollection<ExecuteParameter> source) : this(source == null ? 0 : source.Count)
        {
            foreach(var item in source)
            {
                this.Add(item);
            }
        }


        /// <summary>
        /// 获取或设置指定参数名称的参数内容。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>获取一个 <see cref="ExecuteParameter"/> 的参数实例。</returns>
        public ExecuteParameter this[string name]
        {
            get
            {
                ExecuteParameter value;
                this._innerDict.TryGetValue(name, out value);
                return value;
            }
            set { this._innerDict[name] = value; }
        }

        /// <summary>
        /// 获取指定参数索引的参数内容。
        /// </summary>
        /// <param name="index">参数索引。</param>
        /// <returns>获取一个 <see cref="ExecuteParameter"/> 的参数实例。</returns>
        public ExecuteParameter this[int index] { get { return this._innerDict.Values.ElementAtOrDefault(index); } }
    }
}
