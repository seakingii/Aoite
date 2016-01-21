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

            Assert.NotNull(container.GetService(type));
            Assert.NotNull(container.GetService(itype));
            Assert.NotEqual(container.GetService(type), container.GetService(type));
            Assert.NotEqual(container.GetService(type), container.GetService(itype));
            Assert.IsAssignableFrom(type, container.GetService(itype));
        }

        [Fact(DisplayName = "添加服务 - 智能解析 - 单例模式")]
        public void AddService_TypeBooleanBoolean_Test2()
        {
            var type = typeof(DefaultService1);
            var itype = typeof(IService1);

            var container = new IocContainer();

            container.Add(type, true);
            Assert.NotNull(container.GetService(type));
            Assert.NotNull(container.GetService(itype));
            Assert.Equal(container.GetService(type), container.GetService(type));
            Assert.Equal(container.GetService(type), container.GetService(itype));
            Assert.IsAssignableFrom(type, container.GetService(itype));
        }

        [Fact(DisplayName = "添加服务 - 指定实例")]
        public void AddService_TypeObjectBoolean_Test1()
        {
            var container = new IocContainer();
            container.Add(typeof(IService2), new XService2());

            var childContainer = new IocContainer(container);
            Assert.IsAssignableFrom<XService2>(childContainer.GetService(typeof(IService2)));
        }

        [Fact(DisplayName = "获取服务 - 智能解析")]
        public void GetServiceTest1()
        {
            var itype = typeof(IService1);
            var container = new IocContainer();

            Assert.False(container.Contains(itype));
            Assert.NotNull(container.GetService(itype));
            Assert.False(container.Contains(typeof(IService_NotFound)));
            Assert.Null(container.GetService(typeof(IService_NotFound)));
        }

        [Fact(DisplayName = "获取服务 - 获取顺序1")]
        public void GetServiceTest2()
        {
            var type = typeof(DefaultService1);
            var container = new IocContainer();
            container.Add(type, true);
            var childContainer = new IocContainer(container);
            Assert.Equal(childContainer.GetService(type), container.GetService(type));
        }

        [Fact(DisplayName = "获取服务 - 获取顺序2")]
        public void GetServiceTest3()
        {
            var type = typeof(DefaultService1);
            var container = new IocContainer();
            var childContainer = new IocContainer(container);
            container.Add(type, true);
            childContainer.Add(type, true);
            Assert.NotEqual(childContainer.GetService(type), container.GetService(type));
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
            Assert.IsAssignableFrom<XService2>(childContainer.GetService(itype));
            Assert.IsAssignableFrom<DefaultService2>(container.GetService(itype));
            childContainer.Remove(itype);
            Assert.IsAssignableFrom<DefaultService2>(childContainer.GetService(itype));
            Assert.IsAssignableFrom<DefaultService2>(container.GetService(itype));
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
            Assert.IsAssignableFrom<XService2>(childContainer.GetService(itype));
            Assert.IsAssignableFrom<DefaultService2>(container.GetService(itype));
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
            container.AddValue("A", 15);
            Assert.Equal(15, container.GetValue("A"));

            int index = 0;
            container.AddValue("B", lmp => ++index);
            Assert.Equal(1, container.GetValue("B"));
            Assert.Equal(2, container.GetValue("B"));
            index = 0;

            container.AddValue("C", lmp => ++index, true);

            Assert.Equal(1, container.GetValue("C"));
            Assert.Equal(1, container.GetValue("C"));

            Assert.True(container.ContainsValue("A"));
            container.RemoveValue("A");
            Assert.False(container.ContainsValue("A"));
        }

        [Fact(DisplayName = "添加值 - 父级")]
        public void Value_Test2()
        {
            var container = new IocContainer();
            var childContainer = new IocContainer(container);
            container.AddValue("A", 15);
            Assert.Equal(15, childContainer.GetValue("A"));

            int index = 0;
            container.AddValue("B", lmp => ++index);
            Assert.Equal(1, childContainer.GetValue("B"));
            Assert.Equal(2, childContainer.GetValue("B"));
            index = 0;

            container.AddValue("C", lmp => ++index, true);

            Assert.Equal(1, childContainer.GetValue("C"));
            Assert.Equal(1, childContainer.GetValue("C"));

            Assert.True(childContainer.ContainsValue("A", true));
            container.RemoveValue("A", true);
            Assert.False(childContainer.ContainsValue("A", true));

            index = 0;
            container.AddValue("D", lmp => ++index);
            Assert.Equal(1, childContainer.GetValue("D"));
            Assert.Equal(2, container.GetValue("D"));

        }

        [Fact(DisplayName = "添加值 - 构造函数")]
        public void Value_Test3()
        {
            var parentContainer = new IocContainer();
            var childContainer = new IocContainer(parentContainer);
            parentContainer.AddValue("value1", 1);
            parentContainer.AddValue("value2", "2");
            parentContainer.AddValue("value3", false);

            childContainer.AddValue("value1", 9999);
            childContainer.AddValue("value2", "9999");

            childContainer.Add(typeof(IValueService), typeof(ValueService1));
            parentContainer.Add(typeof(IValueService), typeof(ValueService2));

            var childService = childContainer.GetService(typeof(IValueService)) as IValueService;
            var parentService = parentContainer.GetService(typeof(IValueService)) as IValueService;

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
            container.AddValue(type, "A", 15);
            Assert.Equal(15, container.GetValue(type, "A"));

            int index = 0;
            container.AddValue(type, "B", lmp => ++index);
            Assert.Equal(1, container.GetValue(type, "B"));
            Assert.Equal(2, container.GetValue(type, "B"));
            index = 0;

            container.AddValue(type, "C", lmp => ++index, true);

            Assert.Equal(1, container.GetValue(type, "C"));
            Assert.Equal(1, container.GetValue(type, "C"));

            Assert.True(container.ContainsValue(type, "A"));
            container.RemoveValue(type, "A");
            Assert.False(container.ContainsValue(type, "A"));
        }

        [Fact(DisplayName = "关联类型添加值 - 父级")]
        public void TypeValue_Test2()
        {
            var type = typeof(IValueService);
            var container = new IocContainer();
            var childContainer = new IocContainer(container);
            container.AddValue(type, "A", 15);
            Assert.Equal(15, childContainer.GetValue(type, "A"));

            int index = 0;
            container.AddValue(type, "B", lmp => ++index);
            Assert.Equal(1, childContainer.GetValue(type, "B"));
            Assert.Equal(2, childContainer.GetValue(type, "B"));
            index = 0;

            container.AddValue(type, "C", lmp => ++index, true);

            Assert.Equal(1, childContainer.GetValue(type, "C"));
            Assert.Equal(1, childContainer.GetValue(type, "C"));

            Assert.True(childContainer.ContainsValue(type, "A", true));
            container.RemoveValue(type, "A", true);
            Assert.False(childContainer.ContainsValue(type, "A", true));

            index = 0;
            container.AddValue(type, "D", lmp => ++index);
            Assert.Equal(1, childContainer.GetValue(type, "D"));
            Assert.Equal(2, container.GetValue(type, "D"));

        }

        [Fact(DisplayName = "关联类型添加值 - 构造函数")]
        public void TypeValue_Test3()
        {
            var parentContainer = new IocContainer();
            var childContainer = new IocContainer(parentContainer);
            parentContainer.AddValue("value1", 1);
            parentContainer.AddValue("value2", "2");
            parentContainer.AddValue("value3", false);

            childContainer.AddValue("value1", 9999);
            childContainer.AddValue("value2", "9999");

            childContainer.Add(typeof(IValueService), typeof(ValueService1));
            parentContainer.Add(typeof(IValueService), typeof(ValueService2));

            var childService = childContainer.GetService(typeof(IValueService)) as IValueService;
            var parentService = parentContainer.GetService(typeof(IValueService)) as IValueService;

            Assert.Equal(1, parentService.Value1);
            Assert.Equal("2", parentService.Value2);
            Assert.Equal(false, parentService.Value3);

            Assert.Equal(9999, childService.Value1);
            Assert.Equal("9999", childService.Value2);
            Assert.Equal(false, childService.Value3);

            childContainer.AddValue(typeof(IValueService), "value2", "8888");
            var childService2 = childContainer.GetService(typeof(IValueService)) as IValueService;

            Assert.Equal(9999, childService2.Value1);
            Assert.Equal("9999", childService2.Value2); //- 因为已映射，所以这里的值还是历史值
            Assert.Equal(false, childService2.Value3);

            childContainer.Remove(typeof(IValueService));
            childContainer.Add(typeof(IValueService), typeof(ValueService1));

            var childService3 = childContainer.GetService(typeof(IValueService)) as IValueService;
            Assert.Equal("8888", childService3.Value2);
        }

        #endregion

        #region LastMapping


        [Fact(DisplayName = "后期绑定 - 基础")]
        public void LastMappingTest1()
        {
            var container = new IocContainer();
            container.AddValue("baseValue1", 10);
            container.AddValue("baseValue2", 20);
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
            container.AddValue("baseValue1", 10);
            container.AddValue("baseValue2", 20);
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
