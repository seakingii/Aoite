namespace Aoite.Serialization
{
    /// <summary>
    /// 定义一个可自定义序列化的功能。
    /// </summary>
    public interface ICustomSerializable
    {
        /// <summary>
        /// 反序列化指定字节数组。
        /// </summary>
        /// <param name="reader">反序列化读取器。</param>
        /// <returns>对象实例。</returns>
        object Deserialize(ObjectReader reader);
        /// <summary>
        /// 序列化指定对象。
        /// </summary>
        /// <param name="writer">序列化写入器。</param>
        /// <param name="value">对象。</param>
        void Serialize(ObjectWriter writer, object value);
    }
}
