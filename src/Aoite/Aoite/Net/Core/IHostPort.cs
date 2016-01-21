namespace Aoite.Net
{
    /// <summary>
    /// 定义一个包含主机地址和主机端口的接口。
    /// </summary>
    public interface IHostPort
    {
        /// <summary>
        /// 获取或设置主机地址。
        /// </summary>
        string Host { get; set; }
        /// <summary>
        /// 获取或设置主机端口。
        /// </summary>
        int Port { get; set; }
    }
}