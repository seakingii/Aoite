namespace System.Web
{
    /// <summary>
    /// 定义一个客户端唯一标识的存储功能。
    /// </summary>
    [DefaultMapping(typeof(Webx.SessionIdentityStore))]
    public interface IIdentityStore : Aoite.CommandModel.IUserFactory
    {
        /// <summary>
        /// 设置客户端唯一标识。
        /// </summary>
        /// <param name="identity">客户端的唯一标识。</param>
        void Set(object identity);
        /// <summary>
        /// 获取客户端的唯一标识。
        /// </summary>
        /// <returns>客户端唯一标识，或一个 null 值。</returns>
        object Get();
        /// <summary>
        /// 移除客户端的唯一标识。
        /// </summary>
        void Remove();
    }
}
