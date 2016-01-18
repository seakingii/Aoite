using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// 表示一个表格数据源。
    /// </summary>
    public abstract class PageData
    {
        /// <summary>
        /// 初始化一个 <see cref="PageData{TModel}"/> 类的新实例。
        /// </summary>
        public PageData() { }
        /// <summary>
        /// 获取或设置行的总数。
        /// </summary>
        public long Total { get; set; }
        /// <summary>
        /// 获取行的数据。
        /// </summary>
        /// <returns>行的数据。</returns>
        public abstract Array GetRows();
    }

    /// <summary>
    /// 表示一个表格数据源。
    /// </summary>
    /// <typeparam name="TModel">数据源的行数据类型。</typeparam>
    public class PageData<TModel> : PageData, IEnumerable<TModel>
    {
        /// <summary>
        /// 初始化一个 <see cref="PageData{TModel}"/> 类的新实例。
        /// </summary>
        public PageData() { }

        /// <summary>
        /// 获取指定索引的数据。
        /// </summary>
        /// <param name="index">数据的索引。</param>
        /// <returns>一个数据。</returns>
        public TModel this[int index]
        {
            get
            {
                var rowCount = this.Rows == null ? 0 : this.Rows.Length;
                if(rowCount == 0 || index < 0 || index >= rowCount) throw new ArgumentOutOfRangeException(nameof(index));
                return this.Rows[index];
            }
        }

        /// <summary>
        /// 获取或设置行的数据。
        /// </summary>
        public TModel[] Rows { get; set; }//TODO: array to list

        /// <summary>
        /// 获取行的数据。
        /// </summary>
        /// <returns>行的数据。</returns>
        public override Array GetRows() => this.Rows;

        private IEnumerator<TModel> GetEnumerator()
        {
            if(this.Rows != null)
            {
                foreach(var item in this.Rows)
                {
                    yield return item;
                }
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        IEnumerator<TModel> IEnumerable<TModel>.GetEnumerator() => this.GetEnumerator();
    }
}
