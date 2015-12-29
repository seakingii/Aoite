using System;
using System.Data;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源交互的简单执行命令参数。
    /// </summary>
    [Serializable]
    public class ExecuteParameter : DbValue, IExecuteParameter
    {
        /// <summary>
        /// 获取参数的名称。
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// 初始化一个 <see cref="ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        public ExecuteParameter(string name, object value) : base(value)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            this.Name = name;
        }

        /// <summary>
        /// 指定 <see cref="DbCommand"/>，生成一个 <see cref="DbParameter"/>。
        /// </summary>
        /// <param name="command">一个 <see cref="DbCommand"/>。</param>
        /// <returns>一个已生成的 <see cref="DbParameter"/>。</returns>
        public virtual DbParameter CreateParameter(DbCommand command)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));

            var dbParameter = command.CreateParameter();
            var parameter = dbParameter as IDbDataParameter;
            parameter.ParameterName = this.Name;
            this.Fill(parameter);
            var dbValue = this.Value as IDbValue;
            if(dbValue != null) dbValue.Fill(parameter);

            return dbParameter;
        }

        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>作为此实例副本的新对象。</returns>
        public virtual object Clone()
        {
            return new ExecuteParameter(this.Name, this.Value)
            {
                Direction = this.Direction,
                Precision = this.Precision,
                Scale = this.Scale,
                Size = this.Size,
                Type = this.Type
            };
        }
    }
}
