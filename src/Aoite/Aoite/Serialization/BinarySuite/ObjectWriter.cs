using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aoite.Serialization
{
    /// <summary>
    /// 表示一个提供序列化对象的功能。
    /// </summary>
    public class ObjectWriter : ObjectFormatterBase
    {
        /// <summary>
        /// 初始化一个类 <see cref="ObjectWriter"/> 的新实例。
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <param name="encoding">序列化的编码。</param>
        public ObjectWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
            if(!stream.CanWrite) throw new NotSupportedException("无法写入数据。");
        }

        /// <summary>
        /// 序列化指定的对象。
        /// </summary>
        /// <param name="value">序列化的对象。</param>
        /// <param name="member">序列化的对象成员。</param>
        public void Serialize(object value, System.Reflection.MemberInfo member = null)
        {
            #region Null & DbNull & SuccessfullyResult

            if(value == null)
            {
                this.WriteNull();
                return;
            }
            if(value is DBNull)
            {
                this.WriteDBNull();
                return;
            }
            if(value is SuccessfullyResult)
            {
                this.WriteSuccessfullyResult();
                return;
            }

            #endregion

            #region ValueType

            if(value is ValueType)
            {
                if(value is Enum) value = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), null);

                if(value is Guid) this.WriteGuid((Guid)value);
                else if(value is DateTime) this.WriteDateTime((DateTime)value);
                else if(value is TimeSpan) this.WriteTimeSpan((TimeSpan)value);
                else if(value is Boolean) this.WriteBoolean((Boolean)value);
                else if(value is Byte) this.WriteByte((Byte)value);
                else if(value is SByte) this.WriteSByte((SByte)value);
                else if(value is Char) this.WriteChar((Char)value);
                else if(value is Single) this.WriteSingle((Single)value);
                else if(value is Double) this.WriteDouble((Double)value);
                else if(value is Decimal) this.WriteDecimal((Decimal)value);
                else if(value is Int16) this.WriteInt16((Int16)value);
                else if(value is Int32) this.WriteInt32((Int32)value);
                else if(value is Int64) this.WriteInt64((Int64)value);
                else if(value is UInt16) this.WriteUInt16((UInt16)value);
                else if(value is UInt32) this.WriteUInt32((UInt32)value);
                else if(value is UInt64) this.WriteUInt64((UInt64)value);
                else this.WriteValueTypeObject(value);
                return;
            }

            #endregion

            if(this.TryWriteReference(value)) return;

            #region Simple

            if(value is String)
            {
                this.WriteString((String)value);
                return;
            }

            if(value is StringBuilder)
            {
                this.WriteStringBuilder((StringBuilder)value);
                return;
            }
            if(value is Type)
            {
                this.WriteType((Type)value);
                return;
            }

            #endregion

            #region Array

            if(value is Array)
            {
                if(value is String[]) this.WriteStringArray((String[])value);
                else if(value is Guid[]) this.WriteGuidArray((Guid[])value);
                else if(value is DateTime[]) this.WriteDateTimeArray((DateTime[])value);
                else if(value is TimeSpan[]) this.WriteTimeSpanArray((TimeSpan[])value);
                else if(value is Boolean[]) this.WriteBooleanArray((Boolean[])value);
                else if(value is Byte[]) this.WriteByteArray((Byte[])value);
                else if(value is SByte[]) this.WriteSByteArray((SByte[])value);
                else if(value is Char[]) this.WriteCharArray((Char[])value);
                else if(value is Single[]) this.WriteSingleArray((Single[])value);
                else if(value is Double[]) this.WriteDoubleArray((Double[])value);
                else if(value is Decimal[]) this.WriteDecimalArray((Decimal[])value);
                else if(value is Int16[]) this.WriteInt16Array((Int16[])value);
                else if(value is Int32[]) this.WriteInt32Array((Int32[])value);
                else if(value is Int64[]) this.WriteInt64Array((Int64[])value);
                else if(value is UInt16[]) this.WriteUInt16Array((UInt16[])value);
                else if(value is UInt32[]) this.WriteUInt32Array((UInt32[])value);
                else if(value is UInt64[]) this.WriteUInt64Array((UInt64[])value);
                else if(value is StringBuilder[]) this.WriteStringBuilderArray((StringBuilder[])value);
                else if(value is Type[]) this.WriteTypeArray((Type[])value);
                else if(value.GetType() == Types.ObjectArray) this.WriteObjectArray((Object[])value);
                else
                {
                    this.WriteArray(value as Array);
                }
                return;
            }

            #endregion

            var type = value.GetType();

            var customAttr = type.GetAttribute<SerializableUsageAttribute>()
                ?? (member == null ? null : member.GetAttribute<SerializableUsageAttribute>())
                ?? QuicklySerializer.CustomAttributes.TryGetValue(type);
            if(customAttr != null)
            {
                this.WriteCustom(value, customAttr.FormatterType);
                return;
            }

            #region Result &　Generic

            if(type == Types.Result)
            {
                this.WriteResult((Result)value);
                return;
            }
            if(type == Types.HybridDictionary)
            {
                this.WriteHybridDictionary((System.Collections.Specialized.HybridDictionary)value);
                return;
            }
            if(type.IsGenericType)
            {
                var defineType = type.GetGenericTypeDefinition();
                if(Types.GResult == defineType)
                {
                    this.WriteGResult((Result)value, type);
                    return;
                }
                if(Types.GList == defineType)
                {
                    this.WriteGList((IList)value, type);
                    return;
                }
                if(Types.GConcurrentDictionary == defineType)
                {
                    this.WriteGConcurrentDictionary((IDictionary)value, type);
                    return;
                }
                if(Types.GDictionary == defineType)
                {
                    this.WriteGDictionary((IDictionary)value, type);
                    return;
                }
            }

            #endregion

            this.WriteObject(value, type);
        }
    }
}
