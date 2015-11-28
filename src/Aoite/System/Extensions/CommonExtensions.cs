using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// 定义复制的策略。
    /// </summary>
    public enum CopyToStrategy
    {
        /// <summary>
        /// 默认方式。
        /// </summary>
        Default,
        /// <summary>
        /// 仅限主键方式。
        /// </summary>
        OnlyPrimaryKey,
        /// <summary>
        /// 仅限非主键方式。
        /// </summary>
        ExcludePrimaryKey,
        /// <summary>
        /// 仅限被修改过的值。
        /// </summary>
        OnlyChangeValues,
    }
    /// <summary>
    /// 提供公共的实用工具方法。
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// 尝试将给定值转换为指定的数据类型。
        /// </summary>
        /// <typeparam name="T">要转换的数据类型。</typeparam>
        /// <param name="value">请求类型转换的值。</param>
        /// <returns>返回一个数据类型的值，或一个 null 值。</returns>
        public static T CastTo<T>(this object value)
        {
            return (T)CastTo(value, typeof(T));
        }

        /// <summary>
        /// 尝试将给定值转换为指定的数据类型。
        /// </summary>
        /// <param name="value">请求类型转换的值。</param>
        /// <param name="type">要转换的数据类型。</param>
        /// <returns>返回一个数据类型的值，或一个 null 值。</returns>
        public static object CastTo(this object value, Type type)
        {
            return type.ChangeType(value);
        }

        /// <summary>
        /// 将 <paramref name="target"/> 所有的属性值复制到当前对象。
        /// </summary>
        /// <typeparam name="TSource">源的数据类型。</typeparam>
        /// <typeparam name="TTarget">目标的数据类型。</typeparam>
        /// <param name="source">复制的源对象。</param>
        /// <param name="target">复制的目标对象。</param>
        /// <param name="targetStrategy">复制目标的策略。</param>
        /// <returns>返回 <paramref name="source"/>。</returns>
        public static TSource CopyFrom<TSource, TTarget>(this TSource source, TTarget target, CopyToStrategy targetStrategy = CopyToStrategy.Default)
        {
            return CopyTo<TSource>(target, source, targetStrategy);
        }

        /// <summary>
        /// 将当前对象所有的属性值复制成一个新的 <typeparamref name="TTarget"/> 实例。
        /// </summary>
        /// <typeparam name="TTarget">新的数据类型。</typeparam>
        /// <param name="source">复制的源对象。</param>
        /// <param name="targetStrategy">复制目标的策略。</param>
        /// <returns>返回一个 <typeparamref name="TTarget"/> 的心实例。</returns>
        public static TTarget CopyTo<TTarget>(this object source, CopyToStrategy targetStrategy = CopyToStrategy.Default)
        {
            if(source == null) return default(TTarget);
            TTarget t2 = Activator.CreateInstance<TTarget>();
            return CopyTo(source, t2, targetStrategy);
        }

        /// <summary>
        /// 将当前对象所有的属性值复制到 <paramref name="target"/>。
        /// </summary>
        /// <typeparam name="TTarget">目标的数据类型。</typeparam>
        /// <param name="source">复制的源对象。</param>
        /// <param name="target">复制的目标对象。</param>
        /// <param name="targetStrategy">复制目标的策略。</param>
        /// <returns>返回 <paramref name="target"/>。</returns>
        public static TTarget CopyTo<TTarget>(this object source, TTarget target, CopyToStrategy targetStrategy = CopyToStrategy.Default)
        {
            if(source == null) throw new ArgumentNullException("source");
            if(target == null) throw new ArgumentNullException("target");

            var sMapper = TypeMapper.Create(source.GetType());
            var tMapper = TypeMapper.Create(target.GetType());
            foreach(var sProperty in sMapper.Properties)
            {
                var tProperty = tMapper[sProperty.Name];
                if(tProperty == null) continue;
                if(targetStrategy == CopyToStrategy.OnlyPrimaryKey && !tProperty.IsKey
                    || (targetStrategy == CopyToStrategy.ExcludePrimaryKey && tProperty.IsKey)
                    || !tProperty.Property.CanWrite) continue;

                object sValue = sProperty.GetValue(source);

                if(targetStrategy == CopyToStrategy.OnlyChangeValues) if(object.Equals(sValue, sProperty.TypeDefaultValue)) continue;

                var spType = sProperty.Property.PropertyType;
                var tpType = tProperty.Property.PropertyType;

                if(spType != tpType)
                {
                    if(tpType.IsValueType && sValue == null)
                    {
                        //throw new ArgumentNullException("{0}.{1}".Fmt(sMapper.Type.Name, sProperty.Property.Name), "目标属性 {0}.{1} 不能为 null 值。".Fmt(tMapper.Type.Name, tProperty.Property.Name));
                        continue;
                    }
                    tProperty.SetValue(target, tpType.ChangeType(sValue));
                }
                else tProperty.SetValue(target, sValue);
            }
            return target;
        }

        /// <summary>
        /// 抛出比较结果的错误。
        /// </summary>
        /// <param name="result">比较结果。</param>
        public static void ThrowIfExists(this CompareResult result)
        {
            if(result != null)
                throw new NotSupportedException(result.ToString());
        }

        /// <summary>
        /// 对当前集合的每个元素执行指定操作。
        /// </summary>
        /// <typeparam name="T">集合的数据类型。</typeparam>
        /// <param name="collection">当前集合。</param>
        /// <param name="action">执行的委托。</param>
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if(collection == null) return;
            if(action == null) throw new ArgumentNullException("action");
            foreach(var item in collection) action(item);
        }

        /// <summary>
        /// 对当前集合的每个元素执行指定操作，并返回一个特定的结果集合。
        /// </summary>
        /// <typeparam name="T">集合的数据类型。</typeparam>
        /// <typeparam name="T2">返回的数据类型。</typeparam>
        /// <param name="collection">当前集合。</param>
        /// <param name="func">执行的委托。</param>
        /// <returns>返回一个集合。</returns>
        public static T2[] Each<T, T2>(this IEnumerable<T> collection, Func<T, T2> func)
        {
            if(collection == null) return null;
            if(func == null) throw new ArgumentNullException("action");
            return InnerEach(collection, func).ToArray();
        }
        private static IEnumerable<T2> InnerEach<T, T2>(IEnumerable<T> collection, Func<T, T2> func)
        {
            foreach(var item in collection) yield return func(item);
        }
        /// <summary>
        /// 返回表示当前对象的 <see cref="System.String"/>，如果 <paramref name="obj"/> 是一个 null 值，将返回 <see cref="System.String.Empty"/>。
        /// </summary>
        /// <param name="obj">一个对象。</param>
        /// <returns>返回 <paramref name="obj"/> 的 <see cref="System.String"/> 或 <see cref="System.String.Empty"/>。</returns>
        public static string ToStringOrEmpty(this object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }

        /// <summary>
        /// 尝试释放当前对象使用的所有资源
        /// </summary>
        /// <param name="obj">释放的对象。</param>
        public static void TryDispose(this IDisposable obj)
        {
            if(obj != null) obj.Dispose();
        }

        /// <summary>
        /// 判定指定的二进制值是否包含有效的值。
        /// </summary>
        /// <param name="value">一个二进制值。</param>
        /// <returns>如果包含返回 true，否则返回 false。</returns>
        public static bool HasValue(this BinaryValue value)
        {
            return BinaryValue.HasValue(value);
        }

        /// <summary>
        /// 将指定的金额转换为中文表示。
        /// </summary>
        /// <param name="money">数字表示的金额。</param>
        /// <returns>返回中文表示的金额。</returns>
        public static string ToChinese(this decimal money)
        {
            var s = money.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            var d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", m => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万亿兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }
    }
}
