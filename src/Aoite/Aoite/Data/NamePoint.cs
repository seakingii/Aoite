namespace Aoite.Data
{
    /// <summary>
    /// 定义在转义的名称位置。
    /// </summary>
    public enum NamePoint
    {
        /// <summary>
        /// 表示一个表的名称。
        /// </summary>
        Table,
        /// <summary>
        /// 表示一个字段的名称 <code>FieldName = ValueName</code>。
        /// </summary>
        Field,
        /// <summary>
        /// 表示一个查询命令字符串的参数值名称 <code>FieldName = ValueName</code>。
        /// </summary>
        Value,
        /// <summary>
        /// 表示一个查询命令参数集合的参数名称 <code>Parameters.Add(Parameter, Value)</code>。
        /// </summary>
        Parameter,
    }
}
