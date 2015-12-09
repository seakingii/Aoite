using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;

namespace System.Data
{
    /// <summary>
    /// 表示内存中数据的一个分页表。
    /// </summary>

    [Serializable]
    [DefaultEvent("RowChanging")]
    [DefaultProperty("TableName")]
    [DesignTimeVisible(false)]
    [Editor("Microsoft.VSDesigner.Data.Design.DataTableEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [ToolboxItem(false)]
    [System.Xml.Serialization.XmlSchemaProvider("GetDataTableSchema")]
    public class PageTable : DataTable
    {
        /// <summary>
        /// 初始化一个 <see cref="PageTable"/> 类的新实例。
        /// </summary>
        public PageTable() { }

        /// <summary>
        /// 使用 <see cref="SerializationInfo"/> 和 <see cref="StreamingContext"/> 初始化 <see cref="PageTable"/> 类的新实例。
        /// </summary>
        /// <param name="info">将对象序列化或反序列化所需的数据。</param>
        /// <param name="context">给定序列化流的源和目的地。</param>
        protected PageTable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Total = info.GetInt64("Total");
        }

        /// <summary>
        /// 获取数据的总行数。
        /// </summary>
        public long Total { get; internal set; }

        /// <summary>
        /// 用序列化 <see cref="PageTable"/> 所需的数据填充序列化信息对象。
        /// </summary>
        /// <param name="info">一个 <see cref="SerializationInfo"/> 对象，它包含与 <see cref="PageTable"/> 关联的序列化数据。</param>
        /// <param name="context">一个 <see cref="StreamingContext"/> 对象，它包含与 <see cref="PageTable"/> 关联的序列化流的源和目标。</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Total", Total);
        }
    }
}
