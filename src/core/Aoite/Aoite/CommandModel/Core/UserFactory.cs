using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个执行命令模型的用户工厂。
    /// </summary>
    public class UserFactory : IUserFactory
    {
        Func<IIocContainer, object> _getUserFunc;
        /// <summary>
        /// 指定一个委托，初始化一个 <see cref="Aoite.CommandModel.UserFactory"/> 类的新实例。
        /// </summary>
        /// <param name="getUserFunc">获取用户的委托。</param>
        public UserFactory(Func<IIocContainer, object> getUserFunc)
        {
            if(getUserFunc == null) throw new ArgumentNullException("getUserFunc");
            this._getUserFunc = getUserFunc;
        }

        object IUserFactory.GetUser(IIocContainer container)
        {
            return _getUserFunc(container);
        }
    }
}
