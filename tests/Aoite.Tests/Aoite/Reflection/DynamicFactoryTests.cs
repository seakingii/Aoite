using Aoite.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xunit;

namespace Aoite.Reflection.Tests
{
    public class DynamicFactoryTests
    {

        class Fields
        {
            public class Model
            {
                public string Public;
                private string Private;
                public static string PublicStatic;
            }
            public class Model<T>
            {
                public T Public;
            }
        }

        class Properties
        {
            public class Model
            {
                public string Public { get; set; }
                public string[] PublicArray { get; set; }
                private string Private { get; set; }
                public static string PublicStatic { get; set; }
            }
            public class Model<T>
            {
                public T Public { get; set; }
            }
        }

        class Methods
        {
            public class Model
            {
                public string Public(int x, int y)
                {
                    return Convert.ToString(x + y);
                }
                private string Private(int x, int y)
                {
                    return Convert.ToString(x + y);
                }
                public static string PublicStatic(int x, int y)
                {
                    return Convert.ToString(x + y);
                }

                public string GPublic<T>(T t1, T t2)
                {
                    return t1.ToString() + t2.ToString();
                }

                public int GetMyProperty(ChildModel model, out int myProperty)
                {
                    myProperty = model.MyProperty + 5;
                    return model.MyProperty;
                }
            }
            public class ChildModel
            {
                public int MyProperty { get; set; }
            }
        }
        private FieldInfo CreateGetter<T>(string fieldName)
        {
            return typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
        }
        [Fact]
        public void CreateFieldGetterTest()
        {
            var di = new DynamicInstance(new Fields.Model() { Public = "a" });

            Assert.Equal("a", di.Get("Public"));
            Assert.Null(di.Get("Private"));
            di = new DynamicInstance(new Fields.Model<int>() { Public = 5 });
            Assert.Equal(5, di.Get("Public"));

            di = new DynamicInstance(typeof(Fields.Model));
            Fields.Model.PublicStatic = "p-s-v";
            Assert.Equal("p-s-v", di.Get("PublicStatic"));
        }
        [Fact]
        public void CreateFieldSetterTest()
        {
            var m = new Fields.Model() { Public = "a" };
            var di = new DynamicInstance(m);
            di.Set("Public", "p-s-v-1");
            di.Set("Private", "p-s-v-2");
            Assert.Equal("p-s-v-1", m.Public);
            Assert.Equal("p-s-v-2", di.Get("Private"));

            di = new DynamicInstance(typeof(Fields.Model));
            di.Set("PublicStatic", "p-s-v-3");
            Assert.Equal("p-s-v-3", di.Get("PublicStatic"));

        }
        [Fact]
        public void CreatePropertyGetterTest()
        {
            var di = new DynamicInstance(new Properties.Model() { Public = "a" });

            Assert.Equal("a", di.Get("Public"));
            Assert.Null(di.Get("Private"));
            di = new DynamicInstance(new Properties.Model<int>() { Public = 5 });
            Assert.Equal(5, di.Get("Public"));

            di = new DynamicInstance(typeof(Properties.Model));
            Properties.Model.PublicStatic = "p-s-v";
            Assert.Equal("p-s-v", di.Get("PublicStatic"));
        }
        [Fact]
        public void CreatePropertySetterTest()
        {
            var m = new Properties.Model() { Public = "a" };
            var di = new DynamicInstance(m);
            di.Set("Public", "p-s-v-1");
            di.Set("Private", "p-s-v-2");
            Assert.Equal("p-s-v-1", m.Public);
            Assert.Equal("p-s-v-2", di.Get("Private"));

            di.Set("PublicArray", new string[] { "1", "2", "3" });
            Assert.Equal(new string[] { "1", "2", "3" }, m.PublicArray);

            di = new DynamicInstance(typeof(Properties.Model));
            di.Set("PublicStatic", "p-s-v-3");
            Assert.Equal("p-s-v-3", di.Get("PublicStatic"));

        }

        struct Point2I
        {
            public int x;
            public int y;
        }

        [Fact]
        public void CreateMethodInvokerTest()
        {
            var m = new Methods.Model();
            var di = new DynamicInstance(m);
            Assert.Equal("11", di.Call("Public", 5, 6));
            Assert.Equal("56", di.Call(new Type[] { Types.Int32 }, "GPublic", 5, 6));
            var cm = new Methods.ChildModel() { MyProperty = 100 };
            var args = new object[] { cm, 0 };

            Assert.Equal(100, di.Call("GetMyProperty", args));
            Assert.Equal(105, args[1]);

            Dictionary<string, string> dict = new Dictionary<string, string>();
            di = new DynamicInstance(dict);
            Assert.Equal(false, di.Call("TryGetValue", "a", string.Empty));
            dict.Add("a", "axx");
            args = new object[] { "a", string.Empty };
            Assert.Equal(true, di.Call("TryGetValue", args));
            Assert.Equal("axx", args[1]);

        }



        class Father
        {
            string Field = "2";
            public string Name { get; set; }
            public virtual string Property { get { return "2"; } }
        }
        class Sum : Father
        {
            string Field = "1";
            public override string Property { get { return "1"; } }
        }

        [Fact]
        public void DynamicInstanceTest()
        {
            var di = new DynamicInstance(new Sum());
            Assert.Equal("1", di.Get("Field"));
            Assert.Equal("1", di.Get("Property"));
            di = new DynamicInstance(new Sum(), typeof(Father));
            Assert.Equal("2", di.Get("Field"));
            Assert.Equal("1", di.Get("Property"));
        }

        struct ChildStruct
        {
            public int Field;
        }
        struct MyStruct
        {
            public int Field;
            public string Property { get; set; }
            public ChildStruct ChildStruct;
        }

        [Fact]
        public void StructTest()
        {
            var box = DynamicFactory.CreateInstance(typeof(MyStruct));

            DynamicInstance di = new DynamicInstance(box);
            di.Set("Field", 6);
            di.Set("Property", "AAA");
            di.Set("ChildStruct", new ChildStruct() { Field = 12 });
            var str = (MyStruct)box;
            Assert.Equal(6, str.Field);
            Assert.Equal("AAA", str.Property);
            Assert.Equal(12, str.ChildStruct.Field);

            di = new DynamicInstance(str);
            Assert.Equal(6, di.Get("Field"));
            Assert.Equal("AAA", di.Get("Property"));
            Assert.Equal(12, ((ChildStruct)di.Get("ChildStruct")).Field);
        }

        class CtorClass
        {
            public int X, Y, Z;
            public CtorClass(int x) : this(x, 0) { }
            public CtorClass(int x, int y) : this(x, y, 0) { }
            CtorClass(int x, int y, int z)
            {
                X = x; Y = y; Z = z;
            }
        }
        struct CtorStruct
        {
            public int X, Y, Z;
            public CtorStruct(int x) : this(x, 0) { }
            public CtorStruct(int x, int y) : this(x, y, 0) { }
            CtorStruct(int x, int y, int z)
            {
                X = x; Y = y; Z = z;
            }
        }

        [Fact]
        public void CtorTest()
        {
            var instance = DynamicFactory.CreateInstance<CtorClass>(1);
            Assert.Equal(1, instance.X);

            instance = DynamicFactory.CreateInstance<CtorClass>(1, 2);
            Assert.Equal(2, instance.Y);

            instance = DynamicFactory.CreateInstance<CtorClass>(1, 2, 3);
            Assert.Equal(3, instance.Z);
        }
        [Fact]
        public void CtorTest2()
        {
            var type = typeof(CtorClass);
            var instance = type.CreateConstructorHandler(Types.Int32)(1) as CtorClass;
            Assert.Equal(1, instance.X);

            instance = type.CreateConstructorHandler(Types.Int32, Types.Int32)(1, 2) as CtorClass;
            Assert.Equal(2, instance.Y);

            instance = type.CreateConstructorHandler(Types.Int32, Types.Int32, Types.Int32)(1, 2, 3) as CtorClass;
            Assert.Equal(3, instance.Z);
        }
        [Fact]
        public void CtorTest3()
        {
            var type = typeof(CtorStruct);
            var instance = (CtorStruct)type.CreateConstructorHandler(Types.Int32)(1);
            Assert.Equal(1, instance.X);

            instance = (CtorStruct)type.CreateConstructorHandler(Types.Int32, Types.Int32)(1, 2);
            Assert.Equal(2, instance.Y);

            instance = (CtorStruct)type.CreateConstructorHandler(Types.Int32, Types.Int32, Types.Int32)(1, 2, 3);
            Assert.Equal(3, instance.Z);
        }
        class CtorClass2
        {
            public int X;
            public CtorClass2(int x, out int y)
            {
                X = x;
                y = x * 10;
            }
        }
        [Fact]
        public void CtorTest4()
        {
            var type = typeof(CtorClass2);
            var args = new object[] { 4, 0 };
            var instance = type.CreateConstructorHandler(Types.Int32, Types.Int32.MakeByRefType())(args) as CtorClass2;
            Assert.Equal(4, instance.X);
            Assert.Equal(40, args[1]);
        }

        class NullableClass
        {
            public int? X { get; set; }
        }

        [Fact]
        public void NullableTest()
        {
            var m = new NullableClass();
            var di = new DynamicInstance(m);
            di.Set("X", 5);
            Assert.Equal(m.X, 5);
            di.Set("X", "6");
            Assert.Equal(m.X, 6);
            di.Set("X", null);
            Assert.Null(m.X);
        }
    }
}