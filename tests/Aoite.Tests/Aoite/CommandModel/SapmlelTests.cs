using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.CommandModel
{
    public class SapmlelTests
    {

        const string Nickname1 = "张三";
        const string Nickname2 = "李四";

        private DefaultBalanceService New(object user = null, Action<MockExecutorFactory> callback = null)
        {
            return ServiceFactory.CreateMockService<DefaultBalanceService>(user, callback);
        }

        [Fact]
        public void InitializeTest1()
        {
            const long money = 500L;

            var service = this.New(Nickname1);
            //AddExecutor(f => f.Mock<AddCommand>((context, command) =>
            //{
            //    Assert.Equal(Nickname1, command.Nickname);
            //    Assert.Equal(money, command.Money);
            //}));
            var type = Assert.Throws<TypeInitializationException>(() => service.Initialize(money));
            Assert.IsType<NotSupportedException>(type.InnerException);
        }

        [Fact]
        public void InitializeTest2()
        {
            const long money = 500L;

            var service = this.New(Nickname1, f => f.Mock<AddCommand>((context, command) =>
            {
                Assert.Equal(Nickname1, command.Nickname);
                Assert.Equal(money, command.Money);
            }));

            service.Initialize(money)
                   .ThrowIfFailded();
        }

        [Fact]
        public void GetListsTest1()
        {
            int index = 0;
            var service = this.New(Nickname1, f => f.Mock<GetListsCommand>((context, command) =>
            {
                command.ResultValue = new List<DeptModel>();
                for(int i = 0; i < 10; i++)
                {
                    command.ResultValue.Add(new DeptModel()
                    {
                        Id = ++index,
                        Name = "name" + index
                    });
                }
            }));
            var list1 = service.GetDepts(5).UnsafeValue;
            Assert.Equal(1, list1[0].Id);
            var list2 = service.GetDepts(5).UnsafeValue;
            Assert.Equal(1, list2[0].Id);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(4));
            var list3 = service.GetDepts(5).UnsafeValue;
            Assert.Equal(11, list3[0].Id);
        }

    }

    public class DefaultBalanceService : CommandModelServiceBase
    {
        public Result Initialize(long money)
        {
            this.Execute(new AddCommand()
            {
                Nickname = this.User,
                Money = money,
            });
            return Result.Successfully;
        }

        public Result<List<DeptModel>> GetDepts(long parentId)
        {
            return this.Execute(new GetListsCommand { ParentDeptId = parentId }).ResultValue;
        }

    }
    public abstract class BalanceCommandBase : ICommand
    {
        public string Nickname { get; set; }
    }
    public class AddCommand : BalanceCommandBase
    {
        public long Money { get; set; }
    }

    public class DeptModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    [Cache("Dept#")]
    public class GetListsCommand : ICommand<List<DeptModel>>, ICommandCache
    {
        public long ParentDeptId { get; set; }

        public List<DeptModel> ResultValue { get; set; }

        ICommandCacheStrategy ICommandCache.CreateStrategy(IContext context)
        {
            return new CommandCacheStrategy(this.ParentDeptId.ToString(), TimeSpan.FromSeconds(3), this, context);
        }

        bool ICommandCache.SetCacheValue(object value)
        {
            return (this.ResultValue = value as List<DeptModel>) != null;
        }

        object ICommandCache.GetCacheValue()
        {
            return this.ResultValue;
        }
    }
}
