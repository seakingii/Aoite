using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Serialization
{
    /// <summary>
    /// 表示一个对象序列化的基类。
    /// </summary>
    public abstract class ObjectFormatterBase : IDisposable
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
        /// 初始化一个 <see cref="Aoite.Serialization.ObjectFormatterBase" />类的新实例。"/>
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <param name="encoding">序列化的编码。</param>
        public ObjectFormatterBase(Stream stream, Encoding encoding)
        {
            if(stream == null) throw new ArgumentNullException("stream");
            this.Stream = stream;
            this.Encoding = encoding ?? Encoding.UTF8;
        }

        void IDisposable.Dispose()
        {
            Stream.Dispose();
        }
    }
}
