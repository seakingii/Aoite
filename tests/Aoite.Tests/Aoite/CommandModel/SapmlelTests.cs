using System;
using System.Collections.Generic;
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
            Assert.Throws<NotSupportedException>(() => service.Initialize(money));
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
                command.Result = new List<DeptModel>();
                for(int i = 0; i < 10; i++)
                {
                    command.Result.Add(new DeptModel()
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

        [Fact]
        public void Cache_With_Key_Test()
        {
            var service = this.New(Nickname1, f => f.Mock<UsernameCacheCommand>((context, command) =>
            {
                command.Result = DateTime.Now + "Username" + command.Id;
            }));

            var result = service.GetUsername(1);
            Assert.NotEqual(result, service.GetUsername(2));
            Assert.NotEqual(result, service.GetUsername(3));
            Assert.Equal(result, service.GetUsername(1));
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(4));
            Assert.NotEqual(result, service.GetUsername(1));
        }

        [Fact]
        public void FindDeptName_Test()
        {
            var service = this.New(Nickname1, f => f.Enqueue(c =>
            {
                var command = c as CMD.CMDWhereBase;
                Assert.Equal("Id", command.Where.Parameters[0].Name);
                Assert.Equal(2L, command.Where.Parameters[0].Value);
                Assert.Equal("[Id]=@Id", command.Where.Where);
                return new { Name = "abc" };
            }));

            Assert.Equal("abc", service.FindDeptName(2));
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

        public string FindDeptName(long id)
        {
            var m = this.Bus.FindOne(id, (DeptModel t) => new { t.Name });
            return m.Name;
        }

        public Result<List<DeptModel>> GetDepts(long parentId)
        {
            return this.Execute(new GetListsCommand { ParentDeptId = parentId }).Result;
        }
        public string GetUsername(long id)
        {
            return this.Execute(new UsernameCacheCommand { Id = id }).Result;
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

        public List<DeptModel> Result { get; set; }

        ICommandCacheStrategy ICommandCache.CreateStrategy(IContext context)
        {
            return new CommandCacheStrategy(this.ParentDeptId.ToString(), TimeSpan.FromSeconds(3), this, context);
        }

        bool ICommandCache.SetCacheValue(object value)
        {
            return (this.Result = value as List<DeptModel>) != null;
        }

        object ICommandCache.GetCacheValue()
        {
            return this.Result;
        }
    }

    [Cache("Username#")]
    public class UsernameCacheCommand : ICommand<string>, ICommandCache
    {
        public long Id { get; set; }
        public string Result { get; set; }
        ICommandCacheStrategy ICommandCache.CreateStrategy(IContext context)
        {
            return new CommandCacheStrategy(Id.ToString(), TimeSpan.FromSeconds(3), this, context);
        }

        bool ICommandCache.SetCacheValue(object value)
        {
            return (this.Result = value as string) != null;
        }

        object ICommandCache.GetCacheValue()
        {
            return this.Result;
        }
    }
}
