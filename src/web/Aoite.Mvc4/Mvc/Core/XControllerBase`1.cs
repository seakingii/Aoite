using Aoite.CommandModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    class AoiteAsyncActionInvoker : System.Web.Mvc.Async.AsyncControllerActionInvoker
    {
        private readonly static object Successfully = new { };
        protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue)
        {
            if(!(actionReturnValue is ActionResult))
            {
                if(actionReturnValue is Result)
                {
                    if(actionReturnValue == Result.Successfully)
                    {
                        actionReturnValue = new JsonNetResult(Successfully);
                    }
                    else
                    {
                        var result = actionReturnValue as IValueResult;
                        var value = result.GetValue();
                        if(result.IsSucceed)
                        {
                            actionReturnValue = new JsonNetResult(value == null ? Successfully : new { Value = value });
                        }
                        else actionReturnValue = new JsonNetResult(new { result.Message, result.Status });
                    }
                }
                else
                {
                    actionReturnValue = new JsonNetResult(actionReturnValue);
                }
            }
            return base.CreateActionResult(controllerContext, actionDescriptor, actionReturnValue);
        }
    }
    /// <summary>
    /// 提供用于响应对 ASP.NET MVC 网站所进行的 HTTP 请求的方法。
    /// </summary>
    public abstract class XControllerBase<TUser> : Controller, IContainerProvider
    {
        private readonly static AoiteAsyncActionInvoker DefaultInvoker = new AoiteAsyncActionInvoker();

        /// <summary>
        /// Creates an action invoker.
        /// </summary>
        /// <returns>An action invoker.</returns>
        protected override IActionInvoker CreateActionInvoker()
        {
            return DefaultInvoker;
        }

        /// <summary>
        ///  获取用户。如果当前请求尚未授权，则为 null 值。
        /// </summary>
        public new virtual TUser User { get { return MvcClient.Identity; } }

        /// <summary>
        /// 获取一个值，指示客户端是否已通过授权。
        /// </summary>
        public virtual bool IsAuthorized { get { return MvcClient.IsAuthorized; } }

        /// <summary>
        /// 创建 <see cref="System.Web.Mvc.JsonResult"/> 对象，该对象使用内容类型、内容编码和 JSON 请求行为将指定对象序列化为 JavaScript 对象表示法 (JSON) 格式。
        /// </summary>
        /// <param name="data">要序列化的 JavaScript 对象图。</param>
        /// <param name="contentType">内容类型（MIME 类型）。</param>
        /// <param name="contentEncoding">内容编码。</param>
        /// <param name="behavior">请求的行为。</param>
        /// <returns>The result object that serializes the specified object to JSON format.</returns>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        private readonly static object Successfully = new { };
        /// <summary>
        /// 返回一个成功的结果。
        /// </summary>
        /// <param name="behavior">请求的行为。</param>
        /// <returns>The result object that serializes the specified object to JSON format.</returns>
        protected JsonResult Success(JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            return Json(Successfully, behavior);
        }
        /// <summary>
        /// 返回一个含值且成功的结果。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <param name="value">成功的值。</param>
        /// <param name="behavior">请求的行为。</param>
        /// <returns>The result object that serializes the specified object to JSON format.</returns>
        protected JsonResult Success<TValue>(TValue value, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            return Json(new { value }, behavior);
        }
        /// <summary>
        /// 返回一个错误的结果。
        /// </summary>
        /// <param name="message">错误的描述。</param>
        /// <param name="status">错误的状态。</param>
        /// <param name="behavior">请求的行为。</param>
        /// <returns>The result object that serializes the specified object to JSON format.</returns>
        public JsonResult Faild(string message, int status = ResultStatus.Failed, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            return Json(new { message, status });
        }

        private IIocContainer _Container = Webx.Container;
        /// <summary>
        /// 设置或获取命令模型服务容器。
        /// </summary>
        public IIocContainer Container { get { return this._Container; } set { this._Container = value; } }

        /// <summary>
        /// 获取命令总线。
        /// </summary>
        protected ICommandBus Bus { get { return this._Container.GetService<ICommandBus>(); } }

        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>返回命令模型。</returns>
        protected virtual TCommand Execute<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            return this.Bus.Execute(command, executing, executed);
        }

        /// <summary>
        /// 以异步的方式执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="executing">命令模型执行前发生的方法。</param>
        /// <param name="executed">命令模型执行后发生的方法。</param>
        /// <returns>返回一个异步操作。</returns>
        protected virtual Task<TCommand> ExecuteAsync<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> executing = null
            , CommandExecutedHandler<TCommand> executed = null) where TCommand : ICommand
        {
            return this.Bus.ExecuteAsync(command, executing, executed);
        }

        /// <summary>
        /// 获取一个全局锁的功能，如果获取锁超时将会抛出异常。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="timeout">获取锁的超时设定。</param>
        /// <returns>返回一个锁。</returns>
        protected IDisposable AcquireLock<T>(TimeSpan? timeout = null)
        {
            return this.AcquireLock(typeof(T).FullName, timeout);
        }

        /// <summary>
        /// 获取一个全局锁的功能，如果获取锁超时将会抛出异常。
        /// </summary>
        /// <param name="key">锁的键。</param>
        /// <param name="timeout">获取锁的超时设定。</param>
        /// <returns>返回一个锁。</returns>
        protected virtual IDisposable AcquireLock(string key, TimeSpan? timeout = null)
        {
            return this._Container.GetService<ILockProvider>().Lock(key, timeout);
        }

        /// <summary>
        /// 获取指定数据类型键的原子递增序列。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        protected long Increment<T>(long increment = 1)
        {
            return this.Increment(typeof(T).FullName, increment);
        }

        /// <summary>
        /// 获取指定键的原子递增序列。
        /// </summary>
        /// <param name="key">序列的键。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        protected virtual long Increment(string key, long increment = 1)
        {
            return this._Container.GetService<ICounterProvider>().Increment(key, increment);
        }

        /// <summary>
        /// 开始事务模式。
        /// </summary>
        /// <returns>返回一个事务。</returns>
        protected virtual ITransaction BeginTransaction()
        {
            return new DefaultTransaction();
        }
    }
}
