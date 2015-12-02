using System;
using System.Data;
using System.Data.Common;

namespace Aoite.Dbx
{
    /// <summary>
    /// 表示一个数据源交互的简单执行命令参数。
    /// </summary>
    [Serializable]
    public class ExecuteParameter : IExecuteParameter
    {
        /// <summary>
        /// 获取参数的名称。
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// 获取或设置参数的值。
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// 获取或设置参数的 <see cref="DbType"/>。
        /// </summary>
        public virtual DbType? Type { get; set; }

        /// <summary>
        /// 获取或设置参数的长度。
        /// </summary>
        public virtual int? Size { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。
        /// </summary>
        public virtual ParameterDirection? Direction { get; set; }

        /// <summary>
        /// 获取或设置一个值数值参数的精度。
        /// </summary>
        public virtual byte? Precision { get; set; }

        /// <summary>
        /// 数值参数的小数位数。
        /// </summary>
        public virtual byte? Scale { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        public ExecuteParameter(string name, object value)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// 指定 <see cref="DbCommand"/>，生成一个 <see cref="DbParameter"/>。
        /// </summary>
        /// <param name="command">一个 <see cref="DbCommand"/>。</param>
        /// <returns>返回一个已生成的 <see cref="DbParameter"/>。</returns>
        public virtual DbParameter CreateParameter(DbCommand command)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));

            var dbParameter = command.CreateParameter();

            var parameter = dbParameter as IDbDataParameter;
            parameter.ParameterName = this.Name;
            parameter.Value = this.Value;

            if(this.Type.HasValue) parameter.DbType = this.Type.Value;
            if(this.Size.HasValue) parameter.Size = this.Size.Value;
            if(this.Direction.HasValue) parameter.Direction = this.Direction.Value;
            if(this.Precision.HasValue) parameter.Precision = this.Precision.Value;
            if(this.Scale.HasValue) parameter.Scale = this.Scale.Value;
           
            return dbParameter;
        }
    }
}
