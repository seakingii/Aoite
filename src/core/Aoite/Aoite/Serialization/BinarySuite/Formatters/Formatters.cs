using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;

namespace Aoite.Serialization
{
    internal static class Formatters
    {
        #region Serialize

        public static void InnerWrite(this ObjectWriter writer, FormatterTag tag, byte[] bytes)
        {
            writer.WriteTag(tag);
            writer.Stream.WriteBytes(bytes);
        }

        public static void InnerWrite(this ObjectWriter writer, Decimal value)
        {
            var bits = Decimal.GetBits(value);
            writer.Stream.WriteBytes(BitConverter.GetBytes(bits[0]));
            writer.Stream.WriteBytes(BitConverter.GetBytes(bits[1]));
            writer.Stream.WriteBytes(BitConverter.GetBytes(bits[2]));
            writer.Stream.WriteBytes(BitConverter.GetBytes(bits[3]));
        }

        public static void InnerWrite(this ObjectWriter writer, Int32 value)
        {
            writer.Stream.WriteBytes(BitConverter.GetBytes(value));
        }

        public static void InnerWrite(this ObjectWriter writer, Array array)
        {
            writer.InnerWrite(array.Length);
            foreach(var item in array) writer.Serialize(item);
        }

        public static void InnerWrite(this ObjectWriter writer, Type type)
        {
            writer.WriteStringOrReference(SerializationHelper.SimplifyQualifiedName(type));
        }

        public static void InnerWriteObject(this ObjectWriter writer, object value, Type type, FormatterTag tagType)
        {
            writer.WriteTag(tagType);
            writer.InnerWrite(type);
            SerializableFieldInfo[] fields = SerializationHelper.GetSerializableMembers(type);

            foreach(var field in fields)
            {
                writer.Serialize(field.GetValue(value), field.Field);
            }
        }

        public static void WriteStringOrReference(this ObjectWriter writer, String value)
        {
            if(value == null) writer.WriteNull();
            else if(!writer.TryWriteReference(value)) writer.WriteString(value);
        }
        public static string ReadStringOrReference(this ObjectReader reader)
        {
            return reader.Deserialize() as string;
        }

        public static bool TryWriteReference(this ObjectWriter writer, string value)
        {
            if(value == null) return false;
            value = string.Intern(value);
            var index = writer.ReferenceContainer.IndexOf(value);
            if(index < 0)
            {
                writer.ReferenceContainer.Add(value);
                return false;
            }
            writer.WriteTag(FormatterTag.Reference);
            writer.InnerWrite(index);
            return true;
        }
        public static bool TryWriteReference(this ObjectWriter writer, object value)
        {
            if(value == null) return false;
            var index = writer.ReferenceContainer.IndexOf(value);
            if(index < 0)
            {
                writer.ReferenceContainer.Add(value);
                return false;
            }
            writer.WriteTag(FormatterTag.Reference);
            writer.InnerWrite(index);
            return true;
        }

        #endregion

        #region Deserialize

        public static object ReadReference(this ObjectReader reader)
        {
            var index = reader.ReadInt32();
            return reader.ReferenceContainer[index];
        }

        public static byte[] ReadBuffer(this ObjectReader reader, int length)
        {
            byte[] buffer = new byte[length];
            reader.Stream.Read(buffer, 0, length);
            return buffer;
        }

        #endregion
    }
}
