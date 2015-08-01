﻿#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://Aoite.Reflection.codeplex.com/
#endregion

using System;
using System.Linq;
using System.Reflection;
using Aoite.Reflection;
using Aoite.Reflection.Caching;
using Aoite.Reflection.Emitter;
using Xunit;

namespace Aoite.ReflectionTest.Internal
{

    public class HashCodeTest
    {
        #region Sample Classes
        public class A1
        {
            public int P1 { get; set; }
        }

        public class B1 : A1
        {
            public string P2 { get; set; }
        }

        public class A2
        {
            public int P1 { get; set; }
        }

        public class B2
        {
            public int P1 { get; set; }
            public string P2 { get; set; }
        }

        public class C
        {
            public void M(A1 arg) { }
        }
        #endregion

        [Fact()]
        public void TestHashCodeUniqueness()
        {
            var types = new[] { typeof(A1), typeof(B1), typeof(A2), typeof(B2) };
            var instances = types.Select(t => t.CreateInstance());
            Assert.Equal(types.Length, instances.Select(t => t.GetHashCode()).Distinct().Count());
        }

        [Fact()]
        public void TestCallInfoHashCodeUniqueness()
        {
            var types = new[] { typeof(A1), typeof(B1), typeof(A2), typeof(B2) };
            var infos = types.Select(t => new CallInfo(t, Type.EmptyTypes, Flags.InstanceAnyVisibility, MemberTypes.Property, "P1", Type.EmptyTypes, null, true));
            Assert.Equal(types.Length, infos.Select(ci => ci.GetHashCode()).Distinct().Count());
            infos = types.Select(t => new CallInfo(t, Type.EmptyTypes, Flags.InstanceAnyVisibility, MemberTypes.Property, "P2", Type.EmptyTypes, null, true));
            Assert.Equal(types.Length, infos.Select(ci => ci.GetHashCode()).Distinct().Count());
        }

        [Fact()]
        public void TestCallInfoEqualityForProperties()
        {
            var types = new[] { typeof(A1), typeof(A1) };
            var infos = types.Select(t => new CallInfo(t, null, Flags.StaticInstanceAnyVisibility, MemberTypes.Property, "P1", Type.EmptyTypes, null, true)).ToList();
            Assert.Equal(infos[0].GetHashCode(), infos[1].GetHashCode());
            Assert.Equal(infos[0], infos[1]);
            Assert.True(infos[0].Equals(infos[1]));
            Assert.True(infos[0] == infos[1]);
        }

        [Fact()]
        public void TestCallInfoEqualityForMethods()
        {
            var args = new[] { new A1() };
            var types = new[] { typeof(C), typeof(C) };
            var infos = types.Select(t => new CallInfo(t, null, Flags.StaticInstanceAnyVisibility, MemberTypes.Method, "M", args.ToTypeArray(), null, true)).ToList();
            Assert.Equal(infos[0].GetHashCode(), infos[1].GetHashCode());
            Assert.Equal(infos[0], infos[1]);
            Assert.True(infos[0].Equals(infos[1]));
            Assert.True(infos[0] == infos[1]);
        }

        [Fact()]
        public void TestCache()
        {
            var types = new[] { typeof(A1), typeof(A1) };
            var infos = types.Select(t => new CallInfo(t, null, Flags.StaticInstanceAnyVisibility, MemberTypes.Property, "P1", Type.EmptyTypes, null, true)).ToList();
            var cache = new Cache<CallInfo, object>();
            infos.ForEach(ci => cache.Insert(ci, ci));
            Assert.Equal(1, cache.Count);
            Assert.NotNull(cache.Get(infos[0]));
            Assert.NotNull(cache.Get(infos[1]));
            Assert.Equal(infos[0], cache.Get(infos[0]));
        }

        [Fact()]
        public void TestMapCallInfoHashCodeUniqueness()
        {
            var map1 = GetMapCallInfo(typeof(A1), typeof(A2), "P1");
            var map2 = GetMapCallInfo(typeof(A2), typeof(A1), "P1");
            Assert.NotEqual(map1.GetHashCode(), map2.GetHashCode());
            map1 = GetMapCallInfo(typeof(B1), typeof(B2), "P1");
            map2 = GetMapCallInfo(typeof(B2), typeof(B1), "P1");
            Assert.NotEqual(map1.GetHashCode(), map2.GetHashCode());
            map1 = GetMapCallInfo(typeof(B1), typeof(B2), "P2");
            map2 = GetMapCallInfo(typeof(B2), typeof(B1), "P2");
            Assert.NotEqual(map1.GetHashCode(), map2.GetHashCode());
            map1 = GetMapCallInfo(typeof(B1), typeof(B2), "P1", "P2");
            map2 = GetMapCallInfo(typeof(B2), typeof(B1), "P1", "P2");
            Assert.NotEqual(map1.GetHashCode(), map2.GetHashCode());
        }

        private static MapCallInfo GetMapCallInfo(Type sourceType, Type targetType, params string[] properties)
        {
            return new MapCallInfo(targetType, Type.EmptyTypes, Flags.InstanceAnyVisibility, MemberTypes.Property, "map", Type.EmptyTypes, null,
                                    true, sourceType, MemberTypes.Property, MemberTypes.Property, properties);
        }
    }
}