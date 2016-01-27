using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace System
{
    partial class BinaryValue
    {
        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Boolean"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="Boolean"/> 的新实例。</returns>
        public static implicit operator Boolean(BinaryValue value)
            => HasValue(value) ? BitConverter.ToBoolean(value._ByteArray, 0) : default(Boolean);

        /// <summary>
        /// <see cref="Boolean"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Boolean"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(Boolean value)
            => new BinaryValue(BitConverter.GetBytes(value));

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Char"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="Char"/> 的新实例。</returns>
        public static implicit operator Char(BinaryValue value)
            => HasValue(value) ? BitConverter.ToChar(value._ByteArray, 0) : default(Char);

        /// <summary>
        /// <see cref="Char"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Char"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(Char value)
            => new BinaryValue(BitConverter.GetBytes(value));

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Double"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="Double"/> 的新实例。</returns>
        public static implicit operator Double(BinaryValue value)
            => HasValue(value) ? BitConverter.ToDouble(value._ByteArray, 0) : default(Double);

        /// <summary>
        /// <see cref="Double"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Double"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(Double value)
            => new BinaryValue(BitConverter.GetBytes(value));

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Int16"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="Int16"/> 的新实例。</returns>
        public static implicit operator Int16(BinaryValue value)
            => HasValue(value) ? BitConverter.ToInt16(value._ByteArray, 0) : default(Int16);

        /// <summary>
        /// <see cref="Int16"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Int16"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(Int16 value)
            => new BinaryValue(BitConverter.GetBytes(value));

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Int32"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="Int32"/> 的新实例。</returns>
        public static implicit operator Int32(BinaryValue value)
            => HasValue(value) ? BitConverter.ToInt32(value._ByteArray, 0) : default(Int32);

        /// <summary>
        /// <see cref="Int32"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Int32"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(Int32 value)
            => new BinaryValue(BitConverter.GetBytes(value));

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Int64"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="Int64"/> 的新实例。</returns>
        public static implicit operator Int64(BinaryValue value)
            => HasValue(value) ? BitConverter.ToInt64(value._ByteArray, 0) : default(Int64);

        /// <summary>
        /// <see cref="Int64"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Int64"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(Int64 value)
            => new BinaryValue(BitConverter.GetBytes(value));

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Single"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="Single"/> 的新实例。</returns>
        public static implicit operator Single(BinaryValue value)
            => HasValue(value) ? BitConverter.ToSingle(value._ByteArray, 0) : default(Single);

        /// <summary>
        /// <see cref="Single"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Single"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(Single value)
            => new BinaryValue(BitConverter.GetBytes(value));

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="UInt16"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="UInt16"/> 的新实例。</returns>
        public static implicit operator UInt16(BinaryValue value)
            => HasValue(value) ? BitConverter.ToUInt16(value._ByteArray, 0) : default(UInt16);

        /// <summary>
        /// <see cref="UInt16"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="UInt16"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(UInt16 value)
            => new BinaryValue(BitConverter.GetBytes(value));

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="UInt32"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="UInt32"/> 的新实例。</returns>
        public static implicit operator UInt32(BinaryValue value)
            => HasValue(value) ? BitConverter.ToUInt32(value._ByteArray, 0) : default(UInt32);

        /// <summary>
        /// <see cref="UInt32"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="UInt32"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(UInt32 value)
            => new BinaryValue(BitConverter.GetBytes(value));

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="UInt64"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns><see cref="UInt64"/> 的新实例。</returns>
        public static implicit operator UInt64(BinaryValue value)
            => HasValue(value) ? BitConverter.ToUInt64(value._ByteArray, 0) : default(UInt64);

        /// <summary>
        /// <see cref="UInt64"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="UInt64"/> 的新实例。</param>
        /// <returns>二进制的值。</returns>
        public static implicit operator BinaryValue(UInt64 value)
            => new BinaryValue(BitConverter.GetBytes(value));

    }
}