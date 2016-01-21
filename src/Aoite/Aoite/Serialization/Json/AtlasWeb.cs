using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Serialization.Json
{
    static class AtlasWeb
    {
        public const string JSON_ArrayTypeNotSupported = "数组的反序列化不支持类型“{0}”。";
        public const string JSON_BadEscape = "无法识别的转义序列。";
        public const string JSON_CannotConvertObjectToType = "无法将类型为“{0}”的对象转换为类型“{1}”";
        public const string JSON_CannotCreateListType = "无法创建 {0} 的实例。";
        public const string JSON_CannotSerializeMemberGeneric = "无法序列化类型“{1}”上的成员“{0}”。";
        public const string JSON_CircularReference = "序列化类型为“{0}”的对象时检测到循环引用。";
        public const string JSON_DepthLimitExceeded = "已超出 RecursionLimit。";
        public const string JSON_DeserializerTypeMismatch = "无法将对象图反序列化为类型“{0}”。";
        public const string JSON_DictionaryTypeNotSupported = "字典的序列化/反序列化不支持类型“{0}”，键必须为字符串或对象。";
        public const string JSON_ExpectedOpenBrace = "传入的对象无效，应为“{”。";
        public const string JSON_IllegalPrimitive = "无效的 JSON 基元: {0}。";
        public const string JSON_InvalidArrayEnd = "传入的数组无效，应为“]”。";
        public const string JSON_InvalidArrayExpectComma = "传入的数组无效，应为“,”。";
        public const string JSON_InvalidArrayExtraComma = "传入的数组无效，结尾多出了“,”。";
        public const string JSON_InvalidArrayStart = "传入的数组无效，应为“[”。";
        public const string JSON_InvalidEnumType = "基于 System.Int64 或 System.UInt64 的枚举不是 JSON 可序列化的，因为 JavaScript 不支持必需的精度。";
        public const string JSON_InvalidMaxJsonLength = "值必须为正整数。";
        public const string JSON_InvalidMemberName = "传入的对象无效，应为成员名称。";
        public const string JSON_InvalidObject = "传入的对象无效，应为“:”或“}”。";
        public const string JSON_InvalidRecursionLimit = "RecursionLimit 必须为正整数。";
        public const string JSON_MaxJsonLengthExceeded = "使用 FastJsonSerializer 进行序列化或反序列化时出错。字符串的长度超过了为 maxJsonLength 属性设置的值。";
        public const string JSON_NoConstructor = "没有为类型“{0}”定义无参数的构造函数。";
        public const string JSON_StringNotQuoted = "传入的字符串无效，应为“\\”。";
        public const string JSON_UnterminatedString = "传入了未终止的字符串。";
        public const string JSON_ValueTypeCannotBeNull = "无法将 null 转换为值类型。";


    }
}
