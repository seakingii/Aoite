using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aoite.Serialization
{
    /// <summary>
    /// 表示一个对象序列化的基类。
    /// </summary>
    public abstract class ObjectFormatterBase : ObjectDisposableBase
    {
        /// <summary>
        /// 获取正在序列化的流。
        /// </summary>
        public readonly Stream Stream;
        /// <summary>
        /// 获取序列化的编码。
        /// </summary>
        public readonly Encoding Encoding;
        /// <summary>
        /// 获取引用的对象集合。
        /// </summary>
        internal readonly List<object> ReferenceContainer = new List<object>(11);

        /// <summary>
        /// 初始化一个 <see cref="ObjectFormatterBase" />类的新实例。"/>
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <param name="encoding">序列化的编码。</param>
        public ObjectFormatterBase(Stream stream, Encoding encoding)
        {
            if(stream == null) throw new ArgumentNullException(nameof(stream));
            this.Stream = stream;
            this.Encoding = encoding ?? Encoding.UTF8;
        }


        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            Stream.Dispose();
            base.DisposeManaged();
        }
    }
}
