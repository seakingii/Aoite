using Aoite.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;

namespace Aoite.Redis
{
    class RedisSessionState
    {
        public DateTime Created { get; set; }
        public bool Locked { get; set; }
        public int LockId { get; set; }
        public DateTime LockDate { get; set; }
        public int Timeout { get; set; }

        [SerializableUsage(typeof(Serializable))]
        public SessionStateItemCollection Items { get; set; }
        public SessionStateActions Flags { get; set; }

        internal RedisSessionState()
        {
            this.Items = new SessionStateItemCollection();
            this.Locked = false;
            this.Created = DateTime.UtcNow;
        }
        class Serializable : Aoite.Serialization.ICustomSerializable
        {
            //public object Deserialize(byte[] bytes)
            //{
            //    if(bytes == null || bytes.Length == 0) return new SessionStateItemCollection();
            //    using(var ms = new MemoryStream(bytes))
            //    using(var reader = new BinaryReader(ms, GA.UTF8))
            //    {
            //        return SessionStateItemCollection.Deserialize(reader);
            //    }
            //}

            //public byte[] Serialize(object value)
            //{
            //    if(value == null) return new byte[0];
            //    var item = value as SessionStateItemCollection;
            //    if(item.Count == 0) return new byte[0];
            //    using(var ms = new MemoryStream())
            //    using(var writer = new BinaryWriter(ms, GA.UTF8))
            //    {
            //        item.Serialize(writer);
            //        return ms.ToArray();
            //    }
            //}
            public object Deserialize(ObjectReader reader)
            {
                SessionStateItemCollection items = new SessionStateItemCollection();
                var count = reader.ReadInt32();
                for(int i = 0; i < count; i++)
                {
                    var name = (string)reader.Deserialize();
                    var value = reader.Deserialize();
                    items[name] = value;
                }
                return items;
            }

            public void Serialize(ObjectWriter writer, object value)
            {
                var items = value as SessionStateItemCollection;
                writer.InnerWrite(items.Count);
                foreach(string name in items.Keys)
                {
                    writer.Serialize(name);
                    writer.Serialize(items[name]);
                }
            }
        }
    }

    /// <summary>
    /// 表示一个使用 Redis 作为数据存储区会话状态的提供程序。
    /// </summary>
    /// <example>
    /// web.config 配置：
    /// <code>
    /// <![CDATA[
    ///   <system.web>
    ///     <sessionState mode="Custom" customProvider="RedisSessionStateProvider">
    ///       <providers>
    ///         <clear />
    ///         <add name="RedisSessionStateProvider" 
    ///              type="Aoite.Redis.RedisSessionStateStoreProvider" 
    ///              address="localhost:6379" password="" />
    ///       </providers>
    ///     </sessionState>
    ///   </system.web>
    /// ]]>
    /// </code>
    /// </example>
    public class RedisSessionStateStoreProvider : SessionStateStoreProviderBase
    {
        private readonly Func<HttpContext, HttpStaticObjectsCollection> _staticObjectsGetter;

        TimeSpan _sessionTimeout;
        RedisManager _redisManager;
        /// <summary>
        /// 获取用于会话状态提供程序的 Redis 管理器。
        /// </summary>
        public RedisManager RedisManager { get { return this._redisManager; } }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Redis.RedisSessionStateStoreProvider"/>
        /// </summary>
        public RedisSessionStateStoreProvider()
            : this(SessionStateUtility.GetSessionStaticObjects) { }

        /// <summary>
        /// 获取当前请求的会话标识符的 Redis 键名。
        /// </summary>
        /// <param name="id">当前请求的会话标识符。</param>
        /// <returns>返回一个 Redis 键名。</returns>
        protected virtual string GetRedisKey(string id)
        {
            return this.Name + "::" + id;
        }

        private void UpdateItem(IRedisClient client, string key, RedisSessionState state)
        {
            client.HMSet(key, state);
        }

        internal RedisSessionStateStoreProvider(Func<HttpContext, HttpStaticObjectsCollection> staticObjectsGetter)
        {
            this._staticObjectsGetter = staticObjectsGetter;
        }

        /// <summary>
        /// 初始化提供程序。
        /// </summary>
        /// <param name="name">该提供程序的友好名称。</param>
        /// <param name="config">名称/值对的集合，表示在配置中为该提供程序指定的、提供程序特定的属性。</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if(string.IsNullOrWhiteSpace(name)) name = "AspNetSession";

            var address = config["address"];
            var password = config["password"];
            var defaultAddress = RedisManager.DefaultAddress;
            if(address != null)
            {
                var sp = address.Split(':');
                if(sp.Length != 2) throw new ArgumentException("非法的 Redis 的连接地址 {0}。".Fmt(address));
                defaultAddress = new Net.SocketInfo(sp[0], int.Parse(sp[1]));
            }
            this._redisManager = new RedisManager(defaultAddress, password);

            var sessionConfig = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            this._sessionTimeout = sessionConfig.Timeout;

            base.Initialize(name, config);
        }

        /// <summary>
        /// 由 <see cref="System.Web.SessionState.SessionStateModule"/> 对象调用，以便进行每次请求初始化。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        public override void InitializeRequest(HttpContext context) { }

        /// <summary>
        ///在请求结束时由 <see cref="System.Web.SessionState.SessionStateModule"/> 对象调用。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        public override void EndRequest(HttpContext context) { }

        /// <summary>
        /// 创建要用于当前请求的新 <see cref="System.Web.SessionState.SessionStateStoreData"/> 对象。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        /// <param name="timeout">新 <see cref="System.Web.SessionState.SessionStateStoreData"/> 的会话状态 <see cref="System.Web.SessionState.HttpSessionState.Timeout"/> 值。</param>
        /// <returns>当前请求的新 <see cref="System.Web.SessionState.SessionStateStoreData"/>。</returns>
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return new SessionStateStoreData(new SessionStateItemCollection(),
                this._staticObjectsGetter(context),
                timeout);
        }

        /// <summary>
        /// 将新的会话状态项添加到数据存储区中。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的 <see cref="System.Web.SessionState.HttpSessionState.SessionID"/>。</param>
        /// <param name="timeout">当前请求的会话 <see cref="System.Web.SessionState.HttpSessionState.Timeout"/>。</param>
        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            this._redisManager.AcquireRelease(client => this.UpdateSessionState(client
                , this.GetRedisKey(id)
                , new RedisSessionState()
                {
                    Timeout = timeout,
                    Flags = SessionStateActions.InitializeItem
                }));
        }

        private void UpdateSessionState(IRedisClient client, string key, RedisSessionState state)
        {
            using(var tran = client.BeginTransaction())
            {
                tran.On(tran.HMSet(key, state), r => r.ThrowIfFailded());
                tran.Expire(key, TimeSpan.FromMinutes(state.Timeout));
                tran.Commit();
            }
        }


        /// <summary>
        /// 从会话数据存储区中返回只读会话状态数据。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的 <see cref="System.Web.SessionState.HttpSessionState.SessionID"/>。</param>
        /// <param name="locked">当此方法返回时，如果请求的会话项在会话数据存储区被锁定，请包含一个设置为 true 的布尔值；否则请包含一个设置为 false 的布尔值。</param>
        /// <param name="lockAge">当此方法返回时，请包含一个设置为会话数据存储区中的项锁定时间的 <see cref="System.TimeSpan"/> 对象。</param>
        /// <param name="lockId">当此方法返回时，请包含一个设置为当前请求的锁定标识符的对象。有关锁定标识符的详细信息，请参见 <see cref="System.Web.SessionState.SessionStateStoreProviderBase"/> 类摘要中的“锁定会话存储区数据”。</param>
        /// <param name="actions">当此方法返回时，请包含 <see cref="System.Web.SessionState.SessionStateActions"/> 值之一，指示当前会话是否为未初始化的无 Cookie 会话。</param>
        /// <returns>使用会话数据存储区中的会话值和信息填充的 <see cref="System.Web.SessionState.SessionStateStoreData"/>。</returns>
        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            return this.GetItem(false, context, id, out locked, out lockAge, out lockId, out actions);
        }

        /// <summary>
        /// 从会话数据存储区中返回只读会话状态数据。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的 <see cref="System.Web.SessionState.HttpSessionState.SessionID"/>。</param>
        /// <param name="locked">当此方法返回时，如果请求的会话项在会话数据存储区被锁定，请包含一个设置为 true 的布尔值；否则请包含一个设置为 false 的布尔值。</param>
        /// <param name="lockAge">当此方法返回时，请包含一个设置为会话数据存储区中的项锁定时间的 <see cref="System.TimeSpan"/> 对象。</param>
        /// <param name="lockId">当此方法返回时，请包含一个设置为当前请求的锁定标识符的对象。有关锁定标识符的详细信息，请参见 <see cref="System.Web.SessionState.SessionStateStoreProviderBase"/> 类摘要中的“锁定会话存储区数据”。</param>
        /// <param name="actions">当此方法返回时，请包含 <see cref="System.Web.SessionState.SessionStateActions"/> 值之一，指示当前会话是否为未初始化的无 Cookie 会话。</param>
        /// <returns>使用会话数据存储区中的会话值和信息填充的 <see cref="System.Web.SessionState.SessionStateStoreData"/>。</returns>
        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            return this.GetItem(true, context, id, out locked, out lockAge, out lockId, out actions);
        }

        private SessionStateStoreData GetItem(bool isExclusive, HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            locked = false;
            lockAge = TimeSpan.Zero;
            lockId = null;
            actions = SessionStateActions.None;
            SessionStateStoreData result = null;

            var key = this.GetRedisKey(id);
            var client = this._redisManager.Acquire();
            try
            {
                using(var distributedLock = client.Lock(key))
                {
                    var state = client.HGetAll<RedisSessionState>(key);
                    if(state == null) return null;
                    actions = state.Flags;

                    if(state.Locked)
                    {
                        locked = true;
                        lockId = state.LockId;
                        lockAge = DateTime.UtcNow - state.LockDate;
                        return null;
                    }

                    if(isExclusive)
                    {
                        locked = state.Locked = true;
                        state.LockDate = DateTime.UtcNow;
                        lockAge = TimeSpan.Zero;
                        lockId = ++state.LockId;
                    }

                    state.Flags = SessionStateActions.None;

                    UpdateSessionState(client, key, state);

                    var items = actions == SessionStateActions.InitializeItem ? new SessionStateItemCollection() : state.Items;

                    result = new SessionStateStoreData(items, this._staticObjectsGetter(context), state.Timeout);
                }
            }
            finally
            {
                this._redisManager.Release(client);
            }
            return result;
        }

        /// <summary>
        /// 释放对会话数据存储区中项的锁定。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的会话标识符。</param>
        /// <param name="lockId">当前请求的锁定标识符。</param>
        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            this._redisManager.AcquireRelease(client => this.UpdateSessionStateIfLocked(client, id, (int)lockId, null, (int)this._sessionTimeout.TotalMinutes));
        }

        private void UpdateSessionStateIfLocked(IRedisClient client, string id, int lockId, ISessionStateItemCollection items, int timeout)
        {
            var key = this.GetRedisKey(id);
            using(var distributedLock = client.Lock(key))
            {
                var state = client.HGetAll<RedisSessionState>(key);
                if(state == null) return;
                if(state.Locked && state.LockId == lockId)
                {
                    state.Locked = false;
                    state.LockId = 0;
                    state.LockDate = DateTime.MinValue;
                    if(items != null) state.Items = items as SessionStateItemCollection;
                    state.Timeout = timeout;
                    UpdateSessionState(client, key, state);
                }
            }
        }

        /// <summary>
        /// 删除会话数据存储区中的项数据。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的会话标识符。</param>
        /// <param name="lockId">当前请求的锁定标识符。</param>
        /// <param name="item">表示将从数据存储区中删除的项的 <see cref="System.Web.SessionState.SessionStateStoreData"/>。</param>
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            var key = this.GetRedisKey(id);
            this._redisManager.AcquireRelease(client =>
            {
                using(client.Lock(key))
                {
                    var state = client.HGetAll<RedisSessionState>(key);
                    if(state == null) return;
                    if(state.Locked && state.LockId == (int)lockId)
                    {
                        client.Del(key);
                    }
                }
            });
        }

        /// <summary>
        /// 更新会话数据存储区中的项的到期日期和时间。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的会话标识符。</param>
        public override void ResetItemTimeout(HttpContext context, string id)
        {
            var key = this.GetRedisKey(id);
            this._redisManager.AcquireRelease(client =>
            {
                client.Expire(key, this._sessionTimeout);
            });
        }

        /// <summary>
        /// 使用当前请求中的值更新会话状态数据存储区中的会话项信息，并清除对数据的锁定。
        /// </summary>
        /// <param name="context">当前请求的 <see cref="System.Web.HttpContext"/>。</param>
        /// <param name="id">当前请求的会话标识符。</param>
        /// <param name="item">包含要存储的当前会话值的 <see cref="System.Web.SessionState.SessionStateStoreData"/> 对象。</param>
        /// <param name="lockId">当前请求的锁定标识符。</param>
        /// <param name="newItem">如果为 true，则将会话项标识为新项；如果为 false，则将会话项标识为现有的项。</param>
        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            this._redisManager.AcquireRelease(client =>
            {
                if(newItem)
                {
                    var state = new RedisSessionState()
                    {
                        Items = (SessionStateItemCollection)item.Items,
                        Timeout = item.Timeout,
                    };

                    var key = GetRedisKey(id);
                    this.UpdateSessionState(client, key, state);
                }
                else
                {
                    this.UpdateSessionStateIfLocked(client, id, (int)lockId, item.Items, item.Timeout);
                }
            });
        }

        /// <summary>
        /// 设置对 Global.asax 文件中定义的 Session_OnEnd 事件的 <see cref="System.Web.SessionState.SessionStateItemExpireCallback"/> 委托的引用。
        /// </summary>
        /// <param name="expireCallback">对 Global.asax 文件中定义的 Session_OnEnd 事件的 <see cref="System.Web.SessionState.SessionStateItemExpireCallback"/> 委托。</param>
        /// <returns>如果会话状态存储提供程序支持调用 Session_OnEnd 事件，则为 true；否则为 false。</returns>
        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }

        /// <summary>
        /// 释放由 <see cref="Aoite.Redis.RedisSessionStateStoreProvider"/> 实现使用的所有资源。
        /// </summary>
        public override void Dispose()
        {
            this._redisManager.Dispose();
        }

    }
}
