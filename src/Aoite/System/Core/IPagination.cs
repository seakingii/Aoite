namespace System
{
    /// <summary>
    /// 提供分页接口的实现。
    /// </summary>
    public interface IPagination
    {
        /// <summary>
        /// 获取或设置以 1 起始的页码。
        /// </summary>
        int PageNumber { get; set; }
        /// <summary>
        /// 获取或设置分页大小。默认为 10。
        /// </summary>
        int PageSize { get; set; }
    }
}

