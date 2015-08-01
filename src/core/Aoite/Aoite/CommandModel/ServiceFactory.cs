using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个服务的工厂。
    /// </summary>
    public static class ServiceFactory
    {
        /// <summary>
        /// 创建一个模拟的服务。
        /// </summary>
        /// <typeparam name="TService">服务的类型。</typeparam>
        /// <param name="mockFactoryCallback">模拟的执行器工厂回调函数。</param>
        /// <param name="redisProvider">Redis 提供程序。若为 null 值表示启用基于应用程序域各种提供程序的服务容器。</param>
        /// <returns>返回服务的实例。</returns>
        public static TService CreateMockService<TService>(Action<MockExecutorFactory> mockFactoryCallback = null
            , IRedisProvider redisProvider = null)
            where TService : IContainerProvider, new()
        {
            return CreateMockService<TService>(null, mockFactoryCallback, redisProvider);
        }

        /// <summary>
        /// 创建一个模拟的服务。
        /// </summary>
        /// <typeparam name="TService">服务的类型。</typeparam>
        /// <param name="user">当前已授权的登录用户。</param>
        /// <param name="mockFactoryCallback">模拟的执行器工厂回调函数。</param>
        /// <param name="redisProvider">Redis 提供程序。若为 null 值表示启用基于应用程序域各种提供程序的服务容器。</param>
        /// <returns>返回服务的实例。</returns>
        public static TService CreateMockService<TService>(object user = null
            , Action<CommandModel.MockExecutorFactory> mockFactoryCallback = null
            , IRedisProvider redisProvider = null)
            where TService : IContainerProvider, new()
        {
            var service = new TService();
            service.Container = CreateContainer(user, mockFactoryCallback, redisProvider);
            return service;
        }

        /// <summary>
        /// 创建一个用于命令模型的服务容器。
        /// </summary>
        /// <param name="user">当前已授权的登录用户。</param>
        /// <param name="mockFactoryCallback">模拟的执行器工厂回调函数。</param>
        /// <param name="redisProvider">Redis 提供程序。若为 null 值表示启用基于应用程序域各种提供程序的服务容器。</param>
        /// <returns>返回一个服务容器。</returns>
        public static IIocContainer CreateContainer(object user = null
            , Action<CommandModel.MockExecutorFactory> mockFactoryCallback = null
            , IRedisProvider redisProvider = null)
        {
            return CreateContainer(new UserFactory(c => user), mockFactoryCallback, redisProvider);
        }

        /// <summary>
        /// 创建一个用于命令模型的服务容器。
        /// </summary>
        /// <param name="userFactory">用户工厂。</param>
        /// <param name="mockFactoryCallback">模拟的执行器工厂回调函数。</param>
        /// <param name="redisProvider">Redis 提供程序。若为 null 值表示启用基于应用程序域各种提供程序的服务容器。</param>
        /// <returns>返回一个服务容器。</returns>
        public static IIocContainer CreateContainer(IUserFactory userFactory
            , Action<CommandModel.MockExecutorFactory> mockFactoryCallback = null
            , IRedisProvider redisProvider = null)
        {
            if(userFactory == null) userFactory = new UserFactory(c => null);

            var container = new IocContainer();
            container.AddService<IUserFactory>(userFactory);
            if(redisProvider != null) container.AddService<IRedisProvider>(redisProvider);

            if(mockFactoryCallback != null)
            {
                var executorFactory = new Aoite.CommandModel.MockExecutorFactory(container);
                mockFactoryCallback(executorFactory);
                container.AddService<CommandModel.IExecutorFactory>(executorFactory);
            }
            if(Db.Engine == null)
            {
                Lazy<IDbEngine> lazyEngine = new Lazy<IDbEngine>(() => new Aoite.Data.MsSqlEngine(""));
                container.AddService<IDbEngine>(lmps => Db.Engine == null ? lazyEngine.Value : Db.Context);
            }
            return container;
        }


        internal static dynamic GetUser(this IIocContainer container)
        {
            if(container == null) throw new ArgumentNullException("container");
            var userFactory = container.GetFixedService<IUserFactory>();
            return userFactory == null ? null : userFactory.GetUser(container);
        }
    }
}
