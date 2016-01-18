using System.Text;
using Aoite.Data;

namespace System
{
    /// <summary>
    /// 表示一个 WHERE 的条件参数。
    /// </summary>
    public class WhereParameters
    {
        /// <summary>
        /// 获取或设置 WHERE 的语句。可以为 null 值。
        /// </summary>
        public string Where { get; set; }
        /// <summary>
        /// 获取或设置 WHERE 的参数集合。可以为 null 值。
        /// </summary>
        public ExecuteParameterCollection Parameters { get; set; }

        internal readonly static WhereParameters Empty = new WhereParameters();

        /// <summary>
        /// 初始化一个 <see cref="WhereParameters"/> 类的新实例。
        /// </summary>
        public WhereParameters() { }

        /// <summary>
        /// 初始化一个 <see cref="WhereParameters"/> 类的新实例。
        /// </summary>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">条件表达式的参数集合。</param>
        public WhereParameters(string where, ExecuteParameterCollection ps = null)
        {
            this.Where = where;
            this.Parameters = ps;
        }

        /// <summary>
        /// 将当前的 WHERE 语句拼接到 <paramref name="commandText"/>。
        /// </summary>
        /// <param name="commandText">命令文本。</param>
        /// <returns>拼接后的命令文本。如果存在 WHERE 条件则添加 WHERE 关键字和条件。</returns>
        public string AppendTo(string commandText)
        {
            var whereText = this.Where;
            if(!string.IsNullOrWhiteSpace(whereText)) commandText = string.Concat(commandText, " WHERE ", whereText);
            return commandText;
        }

        /// <summary>
        /// 解析匿名对象参数集合，并用 AND 符拼接 WHERE 语句。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        public static WhereParameters Parse(IDbEngine engine, object objectInstance, string binary = "AND")
            => Parse(engine, new ExecuteParameterCollection(objectInstance), binary);

        /// <summary>
        /// 解析参数集合，并用 AND 符拼接 WHERE 语句。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合。</param>
        /// <param name="binary">二元运算符。</param>
        public static WhereParameters Parse(IDbEngine engine, ExecuteParameterCollection ps = null, string binary = "AND")
            => new WhereParameters(engine.CreateWhere(ps, binary), ps);
    }
}
