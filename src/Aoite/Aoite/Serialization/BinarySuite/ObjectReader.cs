using System;
using System.IO;
using System.Text;

namespace Aoite.Serialization
{
    /// <summary>
    /// 表示一个提供反序列化对象的功能。
    /// </summary>
    public class ObjectReader : ObjectFormatterBase
    {
        /// <summary>
        /// 提供一个默认的 16 位的缓冲区。
        /// </summary>
        internal readonly byte[] DefaultBuffer = new byte[16];

        /// <summary>
        /// 初始化一个类 <see cref="ObjectReader"/> 的新实例。
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <param name="encoding">序列化的编码。</param>
        public ObjectReader(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
            if(!stream.CanRead) throw new NotSupportedException("无法读取数据。");
        }

        /// <summary>
        /// 反序列化下一个对象。
        /// </summary>
        /// <returns>序列化的对象。</returns>
        public object Deserialize()
        {
            var tag = (FormatterTag)this.Stream.ReadByte();
            switch(tag)
            {
                case FormatterTag.Reference: return this.ReadReference();
                case FormatterTag.Null: return null;
                case FormatterTag.SuccessfullyResult: return Result.Successfully;
                case FormatterTag.DBNull: return DBNull.Value;
                case FormatterTag.Guid: return this.ReadGuid();
                case FormatterTag.DateTime: return this.ReadDateTime();
                case FormatterTag.TimeSpan: return this.ReadTimeSpan();
                case FormatterTag.Boolean: return this.ReadBoolean();
                case FormatterTag.Byte: return this.ReadByte();
                case FormatterTag.SByte: return this.ReadSByte();
                case FormatterTag.Char: return this.ReadChar();
                case FormatterTag.Int16: return this.ReadInt16();
                case FormatterTag.Int32: return this.ReadInt32();
                case FormatterTag.Int64: return this.ReadInt64();
                case FormatterTag.UInt16: return this.ReadUInt16();
                case FormatterTag.UInt32: return this.ReadUInt32();
                case FormatterTag.UInt64: return this.ReadUInt64();
                case FormatterTag.Single: return this.ReadSingle();
                case FormatterTag.Double: return this.ReadDouble();
                case FormatterTag.Decimal: return this.ReadDecimal();
                case FormatterTag.ValueTypeObject: return this.ReadValueTypeObject();
            }

            var index = this.ReferenceContainer.Count;
            this.ReferenceContainer.Add(null);
            object value;
            switch(tag)
            {
                case FormatterTag.Result: return this.ReadResult(index);
                case FormatterTag.GResult: return this.ReadGResult(index);
                case FormatterTag.Array: return this.ReadSimpleArray(index);
                case FormatterTag.MultiRankArray: return this.ReadMultiRankArray(index);
                case FormatterTag.GList: return this.ReadGList(index);
                case FormatterTag.GDictionary: return this.ReadGDictionary(index);
                case FormatterTag.GConcurrentDictionary: return this.ReadGConcurrentDictionary(index);
                case FormatterTag.HybridDictionary: return this.ReadHybridDictionary(index);

                case FormatterTag.Custom: return this.ReadCustom(index);

                case FormatterTag.Object: return this.ReadObject(index);
                case FormatterTag.ObjectArray: return this.ReadObjectArray(index);

                case FormatterTag.Type:
                    value = this.ReadType();
                    break;
                case FormatterTag.TypeArray:
                    value = this.ReadTypeArray();
                    break;
                case FormatterTag.GuidArray:
                    value = this.ReadGuidArray();
                    break;
                case FormatterTag.DateTimeArray:
                    value = this.ReadDateTimeArray();
                    break;
                case FormatterTag.TimeSpanArray:
                    value = this.ReadTimeSpanArray();
                    break;
                case FormatterTag.BooleanArray:
                    value = this.ReadBooleanArray();
                    break;
                case FormatterTag.ByteArray:
                    value = this.ReadByteArray();
                    break;
                case FormatterTag.SByteArray:
                    value = this.ReadSByteArray();
                    break;
                case FormatterTag.CharArray:
                    value = this.ReadCharArray();
                    break;
                case FormatterTag.Int16Array:
                    value = this.ReadInt16Array();
                    break;
                case FormatterTag.Int32Array:
                    value = this.ReadInt32Array();
                    break;
                case FormatterTag.Int64Array:
                    value = this.ReadInt64Array();
                    break;
                case FormatterTag.UInt16Array:
                    value = this.ReadUInt16Array();
                    break;
                case FormatterTag.UInt32Array:
                    value = this.ReadUInt32Array();
                    break;
                case FormatterTag.UInt64Array:
                    value = this.ReadUInt64Array();
                    break;
                case FormatterTag.SingleArray:
                    value = this.ReadSingleArray();
                    break;
                case FormatterTag.DoubleArray:
                    value = this.ReadDoubleArray();
                    break;
                case FormatterTag.DecimalArray:
                    value = this.ReadDecimalArray();
                    break;

                case FormatterTag.String:
                    value = this.ReadString();
                    break;
                case FormatterTag.StringArray:
                    value = this.ReadStringArray();
                    break;
                case FormatterTag.StringBuilder:
                    value = this.ReadStringBuilder();
                    break;
                case FormatterTag.StringBuilderArray:
                    value = this.ReadStringBuilderArray();
                    break;

                default:
                    throw new ArgumentException(tag + "：无法识别的标识。");
            }
            this.ReferenceContainer[index] = value;
            return value;
        }
    }
}
