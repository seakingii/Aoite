namespace System
{
    /// <summary>
    /// 表示两个对象的比较的结果。
    /// </summary>
    public class CompareResult
    {
        /// <summary>
        /// 设置或获取对象的名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设置或获取第一个对象的值。
        /// </summary>
        public object Value1 { get; set; }
        /// <summary>
        /// 设置或获取第二个对象的值。
        /// </summary>
        public object Value2 { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="CompareResult"/> 类的新的实例。
        /// </summary>
        public CompareResult() { }

        /// <summary>
        /// 返回比较结果的描述。
        /// </summary>
        /// <returns>返回一个字符串。</returns>
        public override string ToString()
        {
            return "{0} 预期为“{1}”，实际为“{2}”。".Fmt(Name, Value1 ?? "<NULL>", Value2 ?? "<NULL>");
        }
    }
}
