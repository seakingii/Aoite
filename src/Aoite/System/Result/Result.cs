namespace System
{
    /// <summary>
    /// 定义一个可能具有返回值的结果。
    /// </summary>
    public interface IValueResult : IResult
    {
        /// <summary>
        /// 获取结果的值。如果当前结果没有值，将返回 null 值。
        /// </summary>
        /// <returns>一个结果的值或 null 值。</returns>
        object GetValue();
        /// <summary>
        /// 设置结果的值，如果结果没有值，则不执行任何操作。如果值的类型不符合将会抛出异常。
        /// </summary>
        /// <param name="value">设置的值。</param>
        void SetValue(object value);
    }

    /// <summary>
    /// 表示一个结果。
    /// </summary>
    [Serializable]
    public class Result : IResult, IValueResult
    {
        internal const string SuccessedString = "执行成功！";
        internal const string NullValueString = "[null]";
        /// <summary>
        /// 表示成功、且无法修改的结果。
        /// </summary>
        public readonly static Result Successfully = new SuccessfullyResult();

        internal string _Message;
        [NonSerialized, Ignore]
        internal Exception _Exception;
        internal int _Status = ResultStatus.Succeed;

        /// <summary>
        /// 获取或设置执行结果描述错误的信息。
        /// </summary>
        public virtual string Message { get { return this._Message; } set { this.ToFailded(value); } }
        /// <summary>
        /// 获取或设置执行时发生的错误。结果状态 <see cref="ResultStatus.Succeed"/> 时，该值为 null 值。
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Ignore]
        public virtual Exception Exception
        {
            get
            {
                if(this._Exception == null && this.IsFailed)
                    this._Exception = new ResultException(this._Message, this._Status);
                return this._Exception;
            }
            set { this.ToFailded(value); }
        }

        /// <summary>
        /// 获取一个值，表示执行结果是否为失败。
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool IsFailed { get { return this._Status != ResultStatus.Succeed; } }
        /// <summary>
        /// 获取一个值，表示执行结果是否为成功。
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool IsSucceed { get { return this._Status == ResultStatus.Succeed; } }
        /// <summary>
        /// 获取执行的状态码。
        /// </summary>
        public virtual int Status { get { return this._Status; } }

        /// <summary>
        /// 初始化一个 <see cref="Result"/> 类的新实例。
        /// </summary>
        public Result() { }

        /// <summary>
        /// 指定引发的异常和状态码，初始化一个 <see cref="Result"/> 类的新实例。
        /// </summary>
        /// <param name="exception">引发异常的 <see cref="Exception"/>。如果为 null 值，将不会更改返回结果的状态。</param>
        /// <param name="status">结果的状态码。</param>
        public Result(Exception exception, int status = ResultStatus.Failed)
        {
            this.ToFailded(exception, status);
        }

        /// <summary>
        /// 指定描述错误的信息和状态码，初始化一个 <see cref="Result"/> 类的新实例。
        /// </summary>
        /// <param name="mssage">描述错误的信息。如果为 null 值，将不会更改返回结果的状态。</param>
        /// <param name="status">结果的状态码。</param>
        public Result(string mssage, int status = ResultStatus.Failed)
        {
            this.ToFailded(mssage, status);
        }

        /// <summary>
        /// 返回以字符串形式描述的结果。
        /// </summary>
        /// <returns>如果这是一个成功的操作结果，将返回“执行成功！”，否则返回异常的描述信息。</returns>
        public override string ToString()
        {
            if(this.IsSucceed) return Result.SuccessedString;
            return this._Message;
        }

        internal virtual object GetValue() { return null; }

        object IValueResult.GetValue() { return this.GetValue(); }
        void IValueResult.SetValue(object value) { this.SetValue(value); }

        internal virtual void SetValue(object value) { }
        internal virtual Type GetValueType() { return null; }

        #region implicit operator

        /// <summary>
        /// <see cref="Result"/> 和 <see cref="Exception"/> 的隐式转换。
        /// </summary>
        /// <param name="exception">引发异常的 <see cref="Exception"/>。</param>
        /// <returns>表示一个异常的结果。</returns>
        public static implicit operator Result(Exception exception)
        {
            if(exception == null) return Result.Successfully;
            return new Result(exception);
        }

        /// <summary>
        /// <see cref="Result"/> 和 <see cref="String"/> 的隐式转换。
        /// </summary>
        /// <param name="message">描述异常结果的信息。</param>
        /// <returns>表示一个异常的结果。</returns>
        public static implicit operator Result(string message)
        {
            if(message == null) return Result.Successfully;
            return new Result(message);
        }

        /// <summary>
        /// <see cref="String"/> 和 <see cref="Result"/> 的隐式转换。
        /// </summary>
        /// <param name="result">返回结果。</param>
        /// <returns>字符串形式的结果。如果该结果为 null 值，则返回 null 值。</returns>
        public static implicit operator string(Result result)
        {
            if(result == null) return null;
            return result.ToString();
        }

        /// <summary>
        /// <see cref="Boolean"/> 和 <see cref="Result"/> 的隐式转换。
        /// </summary>
        /// <param name="result">返回结果。</param>
        /// <returns>如果结果非空并且状态为成功返回 true，否则返回 false。</returns>
        public static implicit operator Boolean(Result result)
        {
            return result != null && result.IsSucceed;
        }

        #endregion
    }
}
