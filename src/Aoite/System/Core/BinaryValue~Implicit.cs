namespace System
{
    partial class BinaryValue
    {
        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Boolean"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="Boolean"/> 的新实例。</returns>
        public static implicit operator Boolean(BinaryValue value)
        {
            if(!HasValue(value)) return default(Boolean);
            return BitConverter.ToBoolean(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="Boolean"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Boolean"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Boolean value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Char"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="Char"/> 的新实例。</returns>
        public static implicit operator Char(BinaryValue value)
        {
            if(!HasValue(value)) return default(Char);
            return BitConverter.ToChar(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="Char"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Char"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Char value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Double"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="Double"/> 的新实例。</returns>
        public static implicit operator Double(BinaryValue value)
        {
            if(!HasValue(value)) return default(Double);
            return BitConverter.ToDouble(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="Double"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Double"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Double value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Int16"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="Int16"/> 的新实例。</returns>
        public static implicit operator Int16(BinaryValue value)
        {
            if(!HasValue(value)) return default(Int16);
            return BitConverter.ToInt16(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="Int16"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Int16"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Int16 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Int32"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="Int32"/> 的新实例。</returns>
        public static implicit operator Int32(BinaryValue value)
        {
            if(!HasValue(value)) return default(Int32);
            return BitConverter.ToInt32(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="Int32"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Int32"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Int32 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Int64"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="Int64"/> 的新实例。</returns>
        public static implicit operator Int64(BinaryValue value)
        {
            if(!HasValue(value)) return default(Int64);
            return BitConverter.ToInt64(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="Int64"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Int64"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Int64 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="Single"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="Single"/> 的新实例。</returns>
        public static implicit operator Single(BinaryValue value)
        {
            if(!HasValue(value)) return default(Single);
            return BitConverter.ToSingle(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="Single"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="Single"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Single value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="UInt16"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="UInt16"/> 的新实例。</returns>
        public static implicit operator UInt16(BinaryValue value)
        {
            if(!HasValue(value)) return default(UInt16);
            return BitConverter.ToUInt16(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="UInt16"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="UInt16"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(UInt16 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="UInt32"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="UInt32"/> 的新实例。</returns>
        public static implicit operator UInt32(BinaryValue value)
        {
            if(!HasValue(value)) return default(UInt32);
            return BitConverter.ToUInt32(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="UInt32"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="UInt32"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(UInt32 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="BinaryValue"/> 和 <see cref="UInt64"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="UInt64"/> 的新实例。</returns>
        public static implicit operator UInt64(BinaryValue value)
        {
            if(!HasValue(value)) return default(UInt64);
            return BitConverter.ToUInt64(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="UInt64"/> 和 <see cref="BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="UInt64"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(UInt64 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

    }
}