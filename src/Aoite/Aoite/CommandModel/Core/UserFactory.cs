using System;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个执行命令模型的用户工厂。
    /// </summary>
    public class UserFactory : IUserFactory
    {
        readonly Func<IIocContainer, object> _getUserCallback;

        /// <summary>
        /// 指定一个委托，初始化一个 <see cref="UserFactory"/> 类的新实例。
        /// </summary>
        /// <param name="getUserCallback">获取用户的委托。</param>
        public UserFactory(Func<IIocContainer, object> getUserCallback)
        {
            if(getUserCallback == null) throw new ArgumentNullException(nameof(getUserCallback));
            this._getUserCallback = getUserCallback;
        }

        object IUserFactory.GetUser(IIocContainer container)
        {
            return this._getUserCallback(container);
        }
    }
}
