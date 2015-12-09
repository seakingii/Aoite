namespace Aoite.Data
{
    /// <summary>
    /// 数据源查询的操作类型。
    /// </summary>
    public enum ExecuteType
    {
        /// <summary>
        /// 无值查询。
        /// </summary>
        NoQuery = 1,
        /// <summary>
        /// 单值查询。
        /// </summary>
        Scalar = 2,
        /// <summary>
        /// 读取器查询。
        /// </summary>
        Reader = 4,
        /// <summary>
        /// 数据集查询。
        /// </summary>
        DataSet = 8,
        /// <summary>
        /// 数据表查询。
        /// </summary>
        Table = 16
    }
}
