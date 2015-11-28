using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.CommandModel
{
    [Scripts("Member")]
    public class CommandsTests : TestBase
    {
        private Models.Member[] AddMockModels(Action<Models.Member> before = null, Action<Models.Member> after = null)
        {
            return AddMockModels(this.GetRandomNumber(5, 20), before, after);
        }
        private Models.Member[] AddMockModels(int length, Action<Models.Member> before = null, Action<Models.Member> after = null)
        {
            return this.AddMockModels<Models.Member>(length
                , m =>
                {
                    m.Id = 0;
                    if(before != null) before(m);
                }
                , m =>
                {
                    m.Id = this.Context.GetLastIdentity<Models.Member>().UnsafeValue;
                    if(after != null) after(m);
                });
        }

        [Fact(DisplayName = "CMD.Add")]
        public void Add()
        {
            var model = GA.CreateMockModel<Models.Member>();
            model.Id = 0;
            var command = new CMD.Add<Models.Member>(true) { Entity = model };

            Assert.Equal(1, this.Execute(command).ResultValue);
            var r = GA.Compare(model, this.Context.FindOne<Models.Member>(1).UnsafeValue);
            Assert.Null(r);
        }

        [Fact(DisplayName = "CMD.Modify")]
        public void Modify()
        {
            var models = this.AddMockModels();
            var testId = models.RandomOne().Id;

            var aEntity = new { Id = testId, Username = "admin", Password = "TTT" };
            var command = new CMD.Modify<Models.Member> { Entity = aEntity };

            Assert.Equal(1, this.Execute(command).ResultValue);
            var member2 = this.Context.FindOne<Models.Member>(testId).UnsafeValue;

            Assert.Equal(aEntity.Username, member2.Username);
            Assert.Equal(aEntity.Password, member2.Password);
        }

        [Fact(DisplayName = "CMD.ModifyWhere")]
        public void ModifyWhere()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Username = "username" + (index++ % 3));

            var command = new CMD.ModifyWhere<Models.Member>()
            {
                Entity = new { Password = "1", Money = 999 },
                WhereParameters = WhereParameters.Parse(this.Engine, new { username = "username0" })
            };
            Assert.Equal(models.Where(m => m.Username == "username0").Count(), this.Execute(command).ResultValue);
        }

        [Fact(DisplayName = "CMD.Remove")]
        public void Remove()
        {
            var models = this.AddMockModels();
            var testId = models.RandomOne().Id;
            var command = new CMD.Remove<Models.Member> { Entity = testId };

            Assert.Equal(1, this.Execute(command).ResultValue);
            Assert.Equal(models.Length - 1, this.Context.RowCount<Models.Member>().UnsafeValue);
        }
        [Fact(DisplayName = "CMD.Remove - 批量")]
        public void Remove_Mutli()
        {
            var models = this.AddMockModels(10);

            var command = new CMD.Remove<Models.Member> { Entity = models.RandomAny(2).Select(m => m.Id) };

            Assert.Equal(2, this.Execute(command).ResultValue);
            Assert.Equal(8, this.Context.RowCount<Models.Member>().UnsafeValue);
        }


        [Fact(DisplayName = "CMD.RemoveWhere")]
        public void RemoveWhere()
        {
            var models = this.AddMockModels();
            var testId = models.RandomOne().Id;
            var command = new CMD.RemoveWhere<Models.Member> { WhereParameters = new WhereParameters("Id<4") };

            Assert.Equal(3, this.Execute(command).ResultValue);
        }

        [Fact(DisplayName = "CMD.FindOne")]
        public void FindOne()
        {
            var models = this.AddMockModels();
            var model = models.RandomOne();

            var command = new CMD.FindOne<Models.Member>(model.Id);
            Assert.Null(GA.Compare(model, this.Execute(command).ResultValue));
        }
        [Fact(DisplayName = "CMD.FindOneWhere")]
        public void FindOneWhere()
        {
            var models = this.AddMockModels();
            var model = models.RandomOne();

            var command = new CMD.FindOneWhere<Models.Member>()
            {
                WhereParameters = WhereParameters.Parse(this.Engine, new { model.IdCard, model.Email })
            };
            Assert.Null(GA.Compare(model, this.Execute(command).ResultValue));
        }


        [Fact(DisplayName = "CMD.FindAllWhere")]
        public void FindAllWhere1()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Email = "Email" + (index++ % 2));
            var model = models.RandomOne();

            var command = new CMD.FindAllWhere<Models.Member>()
            {
                WhereParameters = WhereParameters.Parse(this.Engine, new { Email = "Email1" })
            };
            Assert.Equal(models.Where(m => m.Email == "Email1").Count(), this.Execute(command).ResultValue.Count);
        }
        [Fact(DisplayName = "CMD.FindAllWhere - View")]
        public void FindAllWhere2()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Email = "Email" + (index++ % 2));
            var model = models.RandomOne();

            var command = new CMD.FindAllWhere<Models.Member, Models.MemberViewModel>()
            {
                WhereParameters = WhereParameters.Parse(this.Engine, new { Email = "Email1" })
            };
            Assert.Equal(models.Where(m => m.Email == "Email1").Count(), this.Execute(command).ResultValue.Count);
        }


        [Fact(DisplayName = "CMD.FindAllPage")]
        public void FindAllPage1()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Email = "Email" + (index++ % 2));
            var model = models.RandomOne();

            var command = new CMD.FindAllPage<Models.Member>()
            {
                WhereParameters = WhereParameters.Parse(this.Engine, new { Email = "Email1" }),
                Page = new Aoite.Data.Pagination(2, 3)
            };
            var query = models.Where(m => m.Email == "Email1");
            var grid = this.Execute(command).ResultValue;

            Assert.Equal(query.Count(), grid.Total);
            Assert.Equal(query.Skip(3).Take(3).Count(), grid.Rows.Length);
        }
        [Fact(DisplayName = "CMD.FindAllPage - View")]
        public void FindAllPage2()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Email = "Email" + (index++ % 2));
            var model = models.RandomOne();

            var command = new CMD.FindAllPage<Models.Member, Models.MemberViewModel>()
            {
                WhereParameters = WhereParameters.Parse(this.Engine, new { Email = "Email1" }),
                Page = new Aoite.Data.Pagination(2, 3)
            };
            var query = models.Where(m => m.Email == "Email1");
            var grid = this.Execute(command).ResultValue;

            Assert.Equal(query.Count(), grid.Total);
            Assert.Equal(query.Skip(3).Take(3).Count(), grid.Rows.Length);
        }

        [Fact(DisplayName = "CMD.Exists")]
        public void Exists()
        {
            var models = this.AddMockModels();
            var model = models.RandomOne();

            var command = new CMD.Exists<Models.Member>()
            {
                KeyValue = model.Id
            };
            Assert.True(this.Execute(command).ResultValue);

            command.KeyName = "Username";
            command.KeyValue = model.Username;
            Assert.True(this.Execute(command).ResultValue);


            command.KeyName = "Username";
            command.KeyValue = "A$$$$$$$$$$";
            Assert.False(this.Execute(command).ResultValue);
        }

        [Fact(DisplayName = "CMD.ExistsWhere")]
        public void ExistsWhere()
        {
            var models = this.AddMockModels();
            var model = models.RandomOne();

            var command = new CMD.ExistsWhere<Models.Member>()
            {
                WhereParameters = WhereParameters.Parse(this.Engine, new { model.Username }),
            };
            Assert.True(this.Execute(command).ResultValue);

            command.WhereParameters = WhereParameters.Parse(this.Engine, new { Username = "A$$$$$$$$$$" });
            Assert.False(this.Execute(command).ResultValue);
        }


        [Fact(DisplayName = "CMD.RowCount")]
        public void RowCount()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Email = "Email" + (index++ % 2));
            var model = models.RandomOne();

            var command = new CMD.RowCount<Models.Member>()
            {
                WhereParameters = WhereParameters.Parse(this.Engine, new { model.Username }),
            };
            Assert.Equal(1, this.Execute(command).ResultValue);

            var query = models.Where(m => m.Email == "Email1");
            command.WhereParameters = WhereParameters.Parse(this.Engine, new { Email = "Email1" });
            Assert.Equal(query.Count(), this.Execute(command).ResultValue);
        }
    }
}
