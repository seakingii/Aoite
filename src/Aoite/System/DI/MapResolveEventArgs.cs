﻿using Aoite.DI;

namespace System
{
    /// <summary>
    /// 表示映射解析的事件参数。
    /// </summary>
    public class MapResolveEventArgs : EventArgs
    {
        private Type _ExpectType;
        /// <summary>
        /// 获取预期的服务类型。
        /// </summary>
        public Type ExpectType { get { return this._ExpectType; } }
        /// <summary>
        /// 获取或设置一个值，表示映射是否采用单例模式。
        /// </summary>
        public bool SingletonMode { get; set; }
        /// <summary>
        /// 获取或设置服务的生命周期。
        /// </summary>
        public ServiceLifetime Litetime { get; set; }

        /// <summary>
        /// 实例的获取方式回调方法。
        /// </summary>
        public InstanceCreatorCallback Callback { get; set; }

        internal MapResolveEventArgs(Type expectType)
        {
            this._ExpectType = expectType;
        }
    }

    /// <summary>
    /// 表示实例创建的委托。
    /// </summary>
    /// <param name="lastMappingValues">后期绑定的参数列表。</param>
    /// <returns>实例。</returns>
    public delegate object InstanceCreatorCallback(params object[] lastMappingValues);
    /// <summary>
    /// 通过 <see cref="Object{T}"/> 获取实例时，动态设置后期映射的参数值数组。
    /// </summary>
    /// <param name="type">当前依赖注入与控制反转的数据类型。</param>
    /// <returns>后期映射的参数值数组。</returns>
    public delegate object[] LastMappingValuesHandler(Type type);
    /// <summary>
    /// 表示映射解析的事件委托。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">事件参数。</param>
    public delegate void MapResolveEventHandler(object sender, MapResolveEventArgs e);
}
