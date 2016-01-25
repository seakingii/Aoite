using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace System
{
    partial class IocContainer
    {
        private readonly static EmbeddedTypeAwareTypeComparer serviceTypeComparer = new EmbeddedTypeAwareTypeComparer();
        /// <summary>
        /// 表示一个缓存列表，映射到指定类型的构造函数的参数名称的实例盒。优先级最高（1）。
        /// </summary>
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, InstanceBox>> CacheTypeName = new ConcurrentDictionary<Type, ConcurrentDictionary<string, InstanceBox>>(serviceTypeComparer);
        /// <summary>
        /// 表示一个缓存列表，映射对应类型的实例盒。优先级中（2）。
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<InstanceBox>> CacheType = new ConcurrentDictionary<Type, List<InstanceBox>>(serviceTypeComparer);
        /// <summary>
        /// 表示一个缓存列表，映射所有类型的构造函数的参数名称的实例盒。优先级最低（3）。
        /// </summary>
        private readonly ConcurrentDictionary<string, InstanceBox> CacheName = new ConcurrentDictionary<string, InstanceBox>(StringComparer.CurrentCultureIgnoreCase);

        private void Map(Type type, params InstanceBox[] boxes)
        {
            var list = new List<InstanceBox>(boxes);

            CacheType.AddOrUpdate(type, t => list, (t, s) => list);
        }

        private void MapRemove(Type type)
        {
            List<InstanceBox> boxes;
            CacheType.TryRemove(type, out boxes);
        }
        private bool MapContains(Type type)
        {
            return CacheType.ContainsKey(type);
        }
        private InstanceBox[] FindInstanceBoxes(Type type)
        {
            List<InstanceBox> boxes;
            if(CacheType.TryGetValue(type, out boxes)) return boxes.ToArray();
            return new InstanceBox[0];
        }
        private InstanceBox FindInstanceBox(Type type)
        {
            return FindInstanceBoxes(type).FirstOrDefault();
        }
        private bool FindInstanceBox(Type type, out InstanceBox instanceBox)
        {
            instanceBox = FindInstanceBox(type);
            return instanceBox != null;
        }

        private void Map(string name, InstanceBox box)
        {
            CacheName[name] = box;
        }
        private void MapRemove(string name)
        {
            InstanceBox box;
            CacheName.TryRemove(name, out box);
        }
        private bool MapContains(string name)
        {
            return CacheName.ContainsKey(name);
        }
        private InstanceBox FindInstanceBox(string name)
        {
            InstanceBox box;
            if(CacheName.TryGetValue(name, out box)) return box;
            return null;
        }

        private void Map(Type type, string name, InstanceBox box)
        {
            ConcurrentDictionary<string, InstanceBox> typeObjectCaches = null;
            typeObjectCaches = CacheTypeName.GetOrAdd(type, (t) => new ConcurrentDictionary<string, InstanceBox>());
            typeObjectCaches[name] = box;
        }
        private void MapRemove(Type type, string name)
        {
            ConcurrentDictionary<string, InstanceBox> typeObjectCaches = null;
            using(GA.Locking(type))
            {
                if(!CacheTypeName.TryGetValue(type, out typeObjectCaches)) return;
                InstanceBox box;
                typeObjectCaches.TryRemove(name, out box);
                if(typeObjectCaches.Count == 0) CacheTypeName.TryRemove(type, out typeObjectCaches);
            }
        }
        private bool MapContains(Type type, string name)
        {
            ConcurrentDictionary<string, InstanceBox> typeObjectCaches = null;
            using(GA.Locking(type))
            {
                if(!CacheTypeName.TryGetValue(type, out typeObjectCaches)) return false;
                return typeObjectCaches.ContainsKey(name);
            }
        }
        private InstanceBox FindInstanceBox(Type type, string name)
        {
            ConcurrentDictionary<string, InstanceBox> typeObjectCaches = null;
            using(GA.Locking(type))
            {
                if(!CacheTypeName.TryGetValue(type, out typeObjectCaches)) return null;
                InstanceBox box;
                if(typeObjectCaches.TryGetValue(name, out box)) return box;
            }
            return null;
        }
    }
}
