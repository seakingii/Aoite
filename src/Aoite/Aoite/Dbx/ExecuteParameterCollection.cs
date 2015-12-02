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
    public class ExecuteParameterCollection : ICollection<IExecuteParameter>
    {
        private readonly static Type ExecuteParameterType = typeof(IExecuteParameter);
        private readonly Dictionary<string, IExecuteParameter> _innerDict;

        /// <summary>
        /// 获取集合中包含的元素数。
        /// </summary>
        public int Count { get { return this._innerDict.Count; } }

        bool ICollection<IExecuteParameter>.IsReadOnly { get { return false; } }


        /// <summary>
        /// 使用默认容量，初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        public ExecuteParameterCollection() : this(4) { }

        /// <summary>
        /// 指定初始容量初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="capacity">集合可包含的初始元素数。</param>
        public ExecuteParameterCollection(int capacity)
        {
            this._innerDict = new Dictionary<string, IExecuteParameter>(capacity, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 指定对象参数集合，初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="objectInstance">对象参数集合实例。</param>
        public ExecuteParameterCollection(object objectInstance) : this()
        {
            this.Parse(objectInstance);
        }

        /// <summary>
        /// 指定参数集合，初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="keysAndValues">应当是 <see cref="string"/> / <see cref="object"/> 的字典集合。</param>
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
        /// 指定参数数组，初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="parameters">参数数组。</param>
        public ExecuteParameterCollection(params IExecuteParameter[] parameters) : this((ICollection<IExecuteParameter>)parameters) { }

        /// <summary>
        /// 指定参数集合，初始化一个 <see cref="ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="parameters">参数集合。</param>
        public ExecuteParameterCollection(ICollection<IExecuteParameter> parameters) : this(parameters == null ? 0 : parameters.Count)
        {
            foreach(var item in parameters)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// 获取指定参数索引的执行参数。
        /// </summary>
        /// <param name="index">参数索引。</param>
        /// <returns>获取一个 <see cref="IExecuteParameter"/> 的实例。</returns>
        public IExecuteParameter this[int index] { get { return this._innerDict.Values.ElementAtOrDefault(index); } }

        /// <summary>
        /// 获取指定参数名称的执行参数。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>获取一个 <see cref="IExecuteParameter"/> 的实例。</returns>
        public IExecuteParameter this[string name]
        {
            get
            {
                IExecuteParameter value;
                this._innerDict.TryGetValue(name, out value);
                return value;
            }
        }

        void ICollection<IExecuteParameter>.Add(IExecuteParameter parameter) => this.Add(parameter);

        /// <summary>
        /// 指定一个 <see cref="IExecuteParameter"/> 实例，添加到集合中。
        /// </summary>
        /// <param name="parameter">要添加的 <see cref="IExecuteParameter"/> 实例。</param>
        public ExecuteParameterCollection Add(IExecuteParameter parameter)
        {
            if(parameter == null) throw new ArgumentNullException(nameof(parameter));
            var name = parameter.Name;
            if(this._innerDict.ContainsKey(name)) throw new ArgumentException("已存在此名称 " + name + " 的参数！", "parameter.Name");
            this._innerDict.Add(name, parameter);
            return this;
        }

        /// <summary>
        /// 指定参数名和参数值，添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        public ExecuteParameterCollection Add(string name, object value) => this.Add(new ExecuteParameter(name, value));

        /// <summary>
        /// 指定解析一个任意对象，添加到集合中。
        /// </summary>
        /// <param name="objectInstance">一个任意对象。</param>
        public ExecuteParameterCollection Parse(object objectInstance)
        {
            if(objectInstance != null) return this;
            if(objectInstance is IExecuteParameter)
            {
                this.Add(objectInstance as IExecuteParameter);
                return this;
            }

            var objType = objectInstance.GetType();
            var mapper = TypeMapper.Create(objType);
            object value;
            foreach(var prop in mapper.Properties)
            {
                if(prop.IsIgnore) continue;

                value = prop.GetValue(objectInstance);
                if(ExecuteParameterType.IsAssignableFrom(prop.Property.PropertyType)) this.Add(value as IExecuteParameter);
                else this.Add(prop.Name, value);
            }

            return this;
        }

        /// <summary>
        /// 移除指定参数名的 <see cref="ExecuteParameter"/> 项。
        /// </summary>
        /// <param name="name">参数名。</param>
        /// <returns>如果已从集合中成功移除项，则为 true；否则为 false。如果在集合中没有找到项，该方法也会返回 false。</returns>
        public bool Remove(string name) => this._innerDict.Remove(name);

        /// <summary>
        /// 移除指定的 <see cref="ExecuteParameter"/> 项。
        /// </summary>
        /// <param name="parameter">要移除的 <see cref="ExecuteParameter"/>。</param>
        /// <returns>如果已从集合中成功移除项，则为 true；否则为 false。如果在集合中没有找到项，该方法也会返回 false。</returns>
        public bool Remove(IExecuteParameter parameter)
        {
            if(parameter == null) throw new ArgumentNullException(nameof(parameter));
            return this.Remove(parameter.Name);
        }

        /// <summary>
        /// 从集合中移除所有项。
        /// </summary>
        public void Clear() => this._innerDict.Clear();

        /// <summary>
        /// 确定集合是否包含特定的参数名。
        /// </summary>
        /// <param name="name">参数名。</param>
        /// <returns>如果在集合中找到项，则为 true；否则为 false。</returns>
        public bool Contains(string name) => this._innerDict.ContainsKey(name);

        /// <summary>
        /// 确定集合是否包含特定的参数 <see cref="IExecuteParameter"/>。
        /// </summary>
        /// <param name="parameter">要查找的 <see cref="IExecuteParameter"/>。</param>
        /// <returns>如果在集合中找到项，则为 true；否则为 false。</returns>
        public bool Contains(IExecuteParameter parameter)
        {
            if(parameter == null) throw new ArgumentNullException(nameof(parameter));
            return this.Contains(parameter.Name);
        }

        /// <summary>
        /// 从特定的 <see cref="Array"/> 索引开始，将集合的元素复制到一个 <see cref="Array"/> 中。
        /// </summary>
        /// <param name="parameters">作为从集合复制的元素的目标位置的一维 <see cref="Array"/>。<see cref="Array"/> 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex"><paramref name="parameters"/> 中从零开始的索引，从此处开始复制。</param>
        void ICollection<IExecuteParameter>.CopyTo(IExecuteParameter[] parameters, int arrayIndex) => this._innerDict.Values.CopyTo(parameters, arrayIndex);

        IEnumerator<IExecuteParameter> IEnumerable<IExecuteParameter>.GetEnumerator() => this._innerDict.Values.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this._innerDict.Values.GetEnumerator();
    }
}
