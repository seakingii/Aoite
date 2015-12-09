namespace System
{
    /// <summary>
    /// 基于页码的分页实现。
    /// </summary>
    public class Pagination : IPagination
    {
        private int _PageNumber;
        /// <summary>
        /// 获取或设置以 1 起始的页码。
        /// </summary>
        public int PageNumber
        {
            get
            {
                return this._PageNumber;
            }
            set
            {
                if(value < 1) value = 1;
                this._PageNumber = value;
            }
        }

        private int _PageSize;
        /// <summary>
        /// 获取或设置分页大小。默认为 10。
        /// </summary>
        public int PageSize
        {
            get
            {
                return this._PageSize;
            }
            set
            {
                if(value < 1) value = 10;
                this._PageSize = value;
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="Pagination"/> 类的新实例。
        /// </summary>
        public Pagination() : this(1) { }
        /// <summary>
        /// 初始化一个 <see cref="Pagination"/> 类的新实例。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        public Pagination(int pageNumber) : this(pageNumber, 10) { }
        /// <summary>
        /// 初始化一个 <see cref="Pagination"/> 类的新实例。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        public Pagination(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
}
