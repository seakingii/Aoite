using System;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源交互的执行命令。
    /// </summary>
    [Serializable]
    public class ExecuteCommand : ICloneable
    {
        /// <summary>
        /// 获取或设置查询命令的 Transact-SQL 查询字符串。
        /// </summary>
        public string Text { get; set; }

        private ExecuteParameterCollection _Parameters;
        /// <summary>
        /// 获取查询命令的参数的键值集合。
        /// </summary>
        public ExecuteParameterCollection Parameters { get { return this._Parameters ?? (this._Parameters = new ExecuteParameterCollection()); } }

        /// <summary>
        /// 获取查询命令的参数的键值集合的元素数。
        /// </summary>
        public int Count { get { return this._Parameters?.Count ?? 0; } }

        #region Cotrs

        /// <summary>
        /// 初始化一个 <see cref="ExecuteCommand"/> 类的新实例。
        /// </summary>
        public ExecuteCommand() : this(null, (ExecuteParameterCollection)null) { }

        /// <summary>
        /// 指定查询字符串和查询参数，初始化一个 <see cref="ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="text">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        public ExecuteCommand(string text) : this(text, (ExecuteParameterCollection)null) { }

        /// <summary>
        /// 指定查询字符串和匿名参数集合实例，初始化一个 <see cref="ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="text">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public ExecuteCommand(string text, object objectInstance) : this(text, new ExecuteParameterCollection(objectInstance)) { }

        /// <summary>
        /// 指定查询字符串和查询参数，初始化一个 <see cref="ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="text">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        public ExecuteCommand(string text, params object[] parameters) : this(text, new ExecuteParameterCollection(parameters)) { }

        /// <summary>
        /// 指定查询字符串和查询参数，初始化一个 <see cref="ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="text">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        public ExecuteCommand(string text, params IExecuteParameter[] parameters) : this(text, new ExecuteParameterCollection(parameters)) { }

        /// <summary>
        /// 指定查询字符串和查询参数，初始化一个 <see cref="ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="text">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        public ExecuteCommand(string text, ExecuteParameterCollection parameters)
        {
            this.Text = text;
            this._Parameters = parameters;
        }

        #endregion

        /// <summary>
        /// 获取指定参数索引的执行参数。
        /// </summary>
        /// <param name="index">参数索引。</param>
        /// <returns>获取一个 <see cref="IExecuteParameter"/> 的实例。</returns>
        public IExecuteParameter this[int index] { get { return this.Parameters[index]; } }

        /// <summary>
        /// 获取指定参数名称的执行参数。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>获取一个 <see cref="IExecuteParameter"/> 的实例。</returns>
        public IExecuteParameter this[string name] { get { return this.Parameters[name]; } }

        /// <summary>
        /// 指定参数名和参数值，添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        public ExecuteCommand Parameter(string name, object value)
        {
            this.Parameters.Add(name, value);
            return this;
        }

        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>作为此实例副本的新对象。</returns>
        public object Clone()
        {
            ExecuteParameterCollection newParameters = null;
            if(this._Parameters != null)
            {
                newParameters = new ExecuteParameterCollection(this._Parameters.Count);
                foreach(var p in this._Parameters)
                {
                    newParameters.Add(p.Clone() as IExecuteParameter);
                }
            }
            return new ExecuteCommand(this.Text, newParameters);
        }

        /// <summary> 
        /// 返回当前查询命令的字符串形式。
        /// </summary>
        public override string ToString()
        {
            return string.Concat(this.Text, this._Parameters?.ToString());
        }

        [Ignore, NonSerialized]
        private ExecutedEventArgs _EventArgs;
        internal ExecutedEventArgs GetEventArgs(ExecuteType type, System.Data.Common.DbCommand dbCommand, object result)
        {
            var e = this._EventArgs ?? (this._EventArgs = new ExecutedEventArgs(this, dbCommand));
            e.ExecuteType = type;
            e.Result = result;
            return e;
        }
    }
}
