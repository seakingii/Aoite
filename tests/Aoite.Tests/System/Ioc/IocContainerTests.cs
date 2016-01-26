using Xunit;

namespace System.Ioc
{
    public class IocContainerTests
    {
        #region Services

        [Fact(DisplayName = "添加服务 - 自动解析类型")]
        public void AddService_TypeBooleanBoolean_Test1()
        {
            var type = typeof(DefaultService1);
            var itype = typeof(IService1);

            var container = new IocContainer();

            container.Add(type);
            //container.Bind(type).Singleton();

            Assert.NotNull(container.Get(type));
            Assert.NotNull(container.Get(itype));
            Assert.NotEqual(container.Get(type), container.Get(type));
            Assert.NotEqual(container.Get(type), container.Get(itype));
            Assert.IsAssignableFrom(type, container.Get(itype));
        }

        [Fact(DisplayName = "添加服务 - 智能解析 - 单例模式")]
        public void AddService_TypeBooleanBoolean_Test2()
        {
            var type = typeof(DefaultService1);
            var itype = typeof(IService1);

            var container = new IocContainer();

            container.Add(type, true);
            Assert.NotNull(container.Get(type));
            Assert.NotNull(container.Get(itype));
            Assert.Equal(container.Get(type), container.Get(type));
            Assert.NotEqual(container.Get(type), container.Get(itype));
            Assert.IsAssignableFrom(type, container.Get(itype));
        }

        [Fact(DisplayName = "添加服务 - 指定实例")]
        public void AddService_TypeObjectBoolean_Test1()
        {
            var container = new IocContainer();
            container.Add(typeof(IService2), new XService2());

            var childContainer = new IocContainer(container);
            Assert.IsAssignableFrom<XService2>(childContainer.Get(typeof(IService2)));
        }

        [Fact(DisplayName = "获取服务 - 智能解析")]
        public void GetServiceTest1()
        {
            var itype = typeof(IService1);
            var container = new IocContainer();

            Assert.False(container.Contains(itype));
            Assert.NotNull(container.Get(itype));
            Assert.False(container.Contains(typeof(IService_NotFound)));
            Assert.Null(container.Get(typeof(IService_NotFound)));
        }

        [Fact(DisplayName = "获取服务 - 获取顺序1")]
        public void GetServiceTest2()
        {
            var type = typeof(DefaultService1);
            var container = new IocContainer();
            container.Add(type, true);
            var childContainer = new IocContainer(container);
            Assert.Equal(childContainer.Get(type), container.Get(type));
        }

        [Fact(DisplayName = "获取服务 - 获取顺序2")]
        public void GetServiceTest3()
        {
            var type = typeof(DefaultService1);
            var container = new IocContainer();
            var childContainer = new IocContainer(container);
            container.Add(type, true);
            childContainer.Add(type, true);
            Assert.NotEqual(childContainer.Get(type), container.Get(type));
        }

        [Fact(DisplayName = "匹配服务")]
        public void ContainsServiceServiceTest1()
        {
            var container = new IocContainer();
            var childContainer = new IocContainer(container);
            var itype = typeof(IService2);
            childContainer.Add(itype, lmp => new XService2(), promote: true);
            Assert.True(childContainer.Contains(itype));
            childContainer.Remove(itype);
            Assert.False(childContainer.Contains(itype));
            Assert.True(childContainer.Contains(itype, true));
        }

        [Fact(DisplayName = "移除服务")]
        public void RemoveServiceTest1()
        {
            var container = new IocContainer();
            var childContainer = new IocContainer(container);
            var itype = typeof(IService2);
            childContainer.Add(itype, lmp => new XService2(), promote: true);
            Assert.True(container.Contains(itype));
            Assert.True(childContainer.Contains(itype));
            container.Add(itype, lmp => new DefaultService2());
            Assert.IsAssignableFrom<XService2>(childContainer.Get(itype));
            Assert.IsAssignableFrom<DefaultService2>(container.Get(itype));
            childContainer.Remove(itype);
            Assert.IsAssignableFrom<DefaultService2>(childContainer.Get(itype));
            Assert.IsAssignableFrom<DefaultService2>(container.Get(itype));
            Assert.False(childContainer.Contains(itype));
        }

        [Fact(DisplayName = "移除服务 -　含父级")]
        public void RemoveServiceTest2()
        {
            var container = new IocContainer();
            var childContainer = new IocContainer(container);
            var itype = typeof(IService2);
            childContainer.Add(itype, lmp => new XService2(), promote: true);
            Assert.True(container.Contains(itype));
            Assert.True(childContainer.Contains(itype));
            container.Add(itype, lmp => new DefaultService2());
            Assert.IsAssignableFrom<XService2>(childContainer.Get(itype));
            Assert.IsAssignableFrom<DefaultService2>(container.Get(itype));
            childContainer.Remove(itype, true);
            Assert.False(container.Contains(itype));
            Assert.False(childContainer.Contains(itype));
        }

        #endregion

        #region Value

        [Fact(DisplayName = "添加值 - 基础")]
        public void Value_Test1()
        {
            var container = new IocContainer();
            container.Add("A", 15);
            Assert.Equal(15, container.Get("A"));

            int index = 0;
            container.Add("B", lmp => ++index);
            Assert.Equal(1, container.Get("B"));
            Assert.Equal(2, container.Get("B"));
            index = 0;

            container.Add("C", lmp => ++index, true);

            Assert.Equal(1, container.Get("C"));
            Assert.Equal(1, container.Get("C"));

            Assert.True(container.Contains("A"));
            container.Remove("A");
            Assert.False(container.Contains("A"));
        }

        [Fact(DisplayName = "添加值 - 父级")]
        public void Value_Test2()
        {
            var container = new IocContainer();
            var childContainer = new IocContainer(container);
            container.Add("A", 15);
            Assert.Equal(15, childContainer.Get("A"));

            int index = 0;
            container.Add("B", lmp => ++index);
            Assert.Equal(1, childContainer.Get("B"));
            Assert.Equal(2, childContainer.Get("B"));
            index = 0;

            container.Add("C", lmp => ++index, true);

            Assert.Equal(1, childContainer.Get("C"));
            Assert.Equal(1, childContainer.Get("C"));

            Assert.True(childContainer.Contains("A", true));
            container.Remove("A", true);
            Assert.False(childContainer.Contains("A", true));

            index = 0;
            container.Add("D", lmp => ++index);
            Assert.Equal(1, childContainer.Get("D"));
            Assert.Equal(2, container.Get("D"));

        }

        [Fact(DisplayName = "添加值 - 构造函数")]
        public void Value_Test3()
        {
            var parentContainer = new IocContainer();
            var childContainer = new IocContainer(parentContainer);
            parentContainer.Add("value1", 1);
            parentContainer.Add("value2", "2");
            parentContainer.Add("value3", false);

            childContainer.Add("value1", 9999);
            childContainer.Add("value2", "9999");

            childContainer.Add(typeof(IValueService), typeof(ValueService1));
            parentContainer.Add(typeof(IValueService), typeof(ValueService2));

            var childService = childContainer.Get(typeof(IValueService)) as IValueService;
            var parentService = parentContainer.Get(typeof(IValueService)) as IValueService;

            Assert.Equal(1, parentService.Value1);
            Assert.Equal("2", parentService.Value2);
            Assert.Equal(false, parentService.Value3);

            Assert.Equal(9999, childService.Value1);
            Assert.Equal("9999", childService.Value2);
            Assert.Equal(false, childService.Value3);

        }

        #endregion

        #region TypeValue

        [Fact(DisplayName = "关联类型添加值 - 基础")]
        public void TypeValue_Test1()
        {
            var type = typeof(IValueService);
            var container = new IocContainer();
            container.Add(type, "A", 15);
            Assert.Equal(15, container.Get(type, "A"));

            int index = 0;
            container.Add(type, "B", lmp => ++index);
            Assert.Equal(1, container.Get(type, "B"));
            Assert.Equal(2, container.Get(type, "B"));
            index = 0;

            container.Add(type, "C", lmp => ++index, true);

            Assert.Equal(1, container.Get(type, "C"));
            Assert.Equal(1, container.Get(type, "C"));

            Assert.True(container.Contains(type, "A"));
            container.Remove(type, "A");
            Assert.False(container.Contains(type, "A"));
        }

        [Fact(DisplayName = "关联类型添加值 - 父级")]
        public void TypeValue_Test2()
        {
            var type = typeof(IValueService);
            var container = new IocContainer();
            var childContainer = new IocContainer(container);
            container.Add(type, "A", 15);
            Assert.Equal(15, childContainer.Get(type, "A"));

            int index = 0;
            container.Add(type, "B", lmp => ++index);
            Assert.Equal(1, childContainer.Get(type, "B"));
            Assert.Equal(2, childContainer.Get(type, "B"));
            index = 0;

            container.Add(type, "C", lmp => ++index, true);

            Assert.Equal(1, childContainer.Get(type, "C"));
            Assert.Equal(1, childContainer.Get(type, "C"));

            Assert.True(childContainer.Contains(type, "A", true));
            container.Remove(type, "A", true);
            Assert.False(childContainer.Contains(type, "A", true));

            index = 0;
            container.Add(type, "D", lmp => ++index);
            Assert.Equal(1, childContainer.Get(type, "D"));
            Assert.Equal(2, container.Get(type, "D"));

        }

        [Fact(DisplayName = "关联类型添加值 - 构造函数")]
        public void TypeValue_Test3()
        {
            var parentContainer = new IocContainer();
            var childContainer = new IocContainer(parentContainer);
            parentContainer.Add("value1", 1);
            parentContainer.Add("value2", "2");
            parentContainer.Add("value3", false);

            childContainer.Add("value1", 9999);
            childContainer.Add("value2", "9999");

            childContainer.Add(typeof(IValueService), typeof(ValueService1));
            parentContainer.Add(typeof(IValueService), typeof(ValueService2));

            var childService = childContainer.Get(typeof(IValueService)) as IValueService;
            var parentService = parentContainer.Get(typeof(IValueService)) as IValueService;

            Assert.Equal(1, parentService.Value1);
            Assert.Equal("2", parentService.Value2);
            Assert.Equal(false, parentService.Value3);

            Assert.Equal(9999, childService.Value1);
            Assert.Equal("9999", childService.Value2);
            Assert.Equal(false, childService.Value3);

            childContainer.Add(typeof(IValueService), "value2", "8888");
            var childService2 = childContainer.Get(typeof(IValueService)) as IValueService;

            Assert.Equal(9999, childService2.Value1);
            Assert.Equal("9999", childService2.Value2); //- 因为已映射，所以这里的值还是历史值
            Assert.Equal(false, childService2.Value3);

            childContainer.Remove(typeof(IValueService));
            childContainer.Add(typeof(IValueService), typeof(ValueService1));

            var childService3 = childContainer.Get(typeof(IValueService)) as IValueService;
            Assert.Equal("8888", childService3.Value2);
        }

        #endregion

        #region LastMapping


        [Fact(DisplayName = "后期绑定 - 基础")]
        public void LastMappingTest1()
        {
            var container = new IocContainer();
            container.Add("baseValue1", 10);
            container.Add("baseValue2", 20);
            container.Add(typeof(LastMapperTestModel));
            var model = container.Get(typeof(LastMapperTestModel), 50, 80) as LastMapperTestModel;

            Assert.Equal(10, model.BaseValue1);
            Assert.Equal(20, model.BaseValue2);
            Assert.Equal(80, model.Value1);
            Assert.Equal(50, model.Value2);
        }

        [Fact(DisplayName = "后期绑定 - 验证")]
        public void LastMappingTest2()
        {
            var container = new IocContainer();
            container.Add("baseValue1", 10);
            container.Add("baseValue2", 20);
            container.Add(typeof(LastMapperTestModel));
            var ex = Assert.Throws<ArgumentException>(() => container.Get(typeof(LastMapperTestModel), 50));
            Assert.Equal("value1", ex.ParamName);
        }
        #endregion

        #region DefaultMapping

        [Fact()]
        public void DefaultMappingTest()
        {
            var container = new IocContainer();
            var service = container.Get<IDefaultMappingService>();
            Assert.IsType<DefaultMappingService2>(service);
            container.DestroyAll();

            container.Add(typeof(IDefaultMappingService));
            service = container.Get<IDefaultMappingService>();
            Assert.IsType<DefaultMappingService2>(service);
            container.DestroyAll();

            container.Add(typeof(IDefaultMappingService), typeof(DefaultMappingService));
            service = container.Get<IDefaultMappingService>();
            Assert.IsType<DefaultMappingService>(service);
        }
        [Fact()]
        public void DefaultMappingTest2()
        {
            var container = new IocContainer();
            var service = container.Get<DefaultMappingCtorService>();
            Assert.IsType<DefaultMappingService2>(service.InnerService);
            container.DestroyAll();

            container.Add(typeof(IDefaultMappingService), typeof(DefaultMappingService));
            service = container.Get<DefaultMappingCtorService>();
            Assert.IsType<DefaultMappingService>(service.InnerService);
        }
        #endregion
    }
}
