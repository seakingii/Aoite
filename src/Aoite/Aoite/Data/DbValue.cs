using System;
using System.Data;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据库参数的值。
    /// </summary>
    [Serializable]
    public class DbValue : IDbValue
    {
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
        /// 将当前参数配置填充到指定 <see cref="IDbDataParameter"/> 的实例。
        /// </summary>
        /// <param name="parameter">一个 <see cref="IDbDataParameter"/> 的实例。</param>
        public virtual void Fill(IDbDataParameter parameter)
        {
            if(parameter == null) throw new ArgumentNullException(nameof(parameter));

            parameter.Value = this.Value;

            if(this.Type.HasValue) parameter.DbType = this.Type.Value;
            if(this.Size.HasValue) parameter.Size = this.Size.Value;
            if(this.Direction.HasValue) parameter.Direction = this.Direction.Value;
            if(this.Precision.HasValue) parameter.Precision = this.Precision.Value;
            if(this.Scale.HasValue) parameter.Scale = this.Scale.Value;
        }
    }
}
