using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.CommandModel.Core
{
    [Scripts("Member")]
    public class CommandBusPipeTests : TestBase
    {

        private MyServices New(object user = null, Action<MockExecutorFactory> callback = null)
        {
            var service = ServiceFactory.CreateMockService<MyServices>(user, callback);
            service.Container.Add<Aoite.Data.IDbEngine>(this.Engine);
            return service;
        }

        [Fact]
        public async void AddDbTests()
        {
            var s = this.New();
            var model = GA.CreateMockModel<Models.Member>();
            model.Id = 0;
            Assert.True(await s.Add(model));
            Assert.Equal(1, await s.Count());
            Assert.False(await s.Add(model));
        }

        [Fact]
        public async void AddTest()
        {
            var s = ServiceFactory.CreateMockService<MyServices>(null, f =>
                f.Mock(c => false)
                 .Mock(c => 1)
                 .Mock(c => 1)
                 .Mock(c => true)
            );

            var model = GA.CreateMockModel<Models.Member>();
            model.Id = 0;
            Assert.True(await s.Add(model));
            Assert.Equal(1, await s.Count());
            Assert.False(await s.Add(model));
        }
    }

    public class MyServices : CommandModelServiceBase
    {

        public async Task<bool> Add(Models.Member member)
        {
            using(var pipe = Bus.PipeTransaction)
            {
                if(await pipe.ExistsAsync<Models.Member>("email", member.Email)) return false;
                await pipe.AddAsync(member);
                pipe.Commit();
                return true;
            }
        }

        public Task<long> Count()
        {
            return Bus.RowCountAsync<Models.Member>();
        }
    }
}
