using System.Data;

namespace Aoite.Data
{

    /// <summary>
    /// 定义一个数据库参数的值。
    /// </summary>
    public interface IDbValue
    {
        /// <summary>
        /// 获取或设置参数的值。
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// 获取或设置参数的 <see cref="DbType"/>。
        /// </summary>
        DbType? Type { get; set; }

        /// <summary>
        /// 获取或设置参数的长度。
        /// </summary>
        int? Size { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。
        /// </summary>
        ParameterDirection? Direction { get; set; }

        /// <summary>
        /// 获取或设置一个值数值参数的精度。
        /// </summary>
        byte? Precision { get; set; }

        /// <summary>
        /// 数值参数的小数位数。
        /// </summary>
        byte? Scale { get; set; }

        /// <summary>
        /// 将当前参数配置填充到指定 <see cref="IDbDataParameter"/> 的实例。
        /// </summary>
        /// <param name="parameter">一个 <see cref="IDbDataParameter"/> 的实例。</param>
        void Fill(IDbDataParameter parameter);
    }
}
