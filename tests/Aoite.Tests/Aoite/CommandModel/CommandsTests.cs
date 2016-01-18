using System;
using System.Collections.Generic;
using System.Linq;
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
                    m.Id = this.Context.GetLastIdentity<Models.Member>();
                    if(after != null) after(m);
                });
        }

        [Fact(DisplayName = "CMD.Add")]
        public void Add()
        {
            var model = GA.CreateMockModel<Models.Member>();
            model.Id = 0;
            var command = new CMD.Add<Models.Member> { IsIdentityKey = true, Entity = model };

            Assert.Equal(1, this.Execute(command).ResultValue);
            var r = GA.Compare(model, this.Context.FindOne<Models.Member>(1));
            Assert.Null(r);
        }

        [Fact(DisplayName = "CMD.Modify")]
        public void Modify()
        {
            var models = this.AddMockModels();
            var testId = models.RandomOne().Id;

            var aEntity = new { Username = "admin", Password = "TTT" };
            var command = new CMD.Modify<Models.Member> { Entity = aEntity, Where = new WhereParameters("Id=@id", new Data.ExecuteParameterCollection("@id", testId)) };

            Assert.Equal(1, this.Execute(command).ResultValue);
            var member2 = this.Context.FindOne<Models.Member>(testId);

            Assert.Equal(aEntity.Username, member2.Username);
            Assert.Equal(aEntity.Password, member2.Password);
        }

        [Fact(DisplayName = "CMD.ModifyWhere")]
        public void ModifyWhere()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Username = "username" + (index++ % 3));

            var command = new CMD.Modify<Models.Member>()
            {
                Entity = new { Password = "1", Money = 999 },
                Where = WhereParameters.Parse(this.Engine, new { username = "username0" })
            };
            Assert.Equal(models.Where(m => m.Username == "username0").Count(), this.Execute(command).ResultValue);
        }

        [Fact(DisplayName = "CMD.Remove")]
        public void Remove()
        {
            var models = this.AddMockModels();
            var testId = models.RandomOne().Id;
            var command = new CMD.Remove<Models.Member> { Where = this.Engine.GetRemoveWhere<Models.Member>(testId) };

            Assert.Equal(1, this.Execute(command).ResultValue);
            Assert.Equal(models.Length - 1, this.Context.RowCount<Models.Member>());
        }
        [Fact(DisplayName = "CMD.Remove - 批量")]
        public void Remove_Mutli()
        {
            var models = this.AddMockModels(10);

            var command = new CMD.Remove<Models.Member> { Where = this.Engine.GetRemoveWhere<Models.Member>(models.RandomAny(2).Select(m => m.Id)) };

            Assert.Equal(2, this.Execute(command).ResultValue);
            Assert.Equal(8, this.Context.RowCount<Models.Member>());
        }


        [Fact(DisplayName = "CMD.RemoveWhere")]
        public void RemoveWhere()
        {
            var models = this.AddMockModels();
            var testId = models.RandomOne().Id;
            var command = new CMD.Remove<Models.Member> { Where = new WhereParameters("Id<4") };

            Assert.Equal(3, this.Execute(command).ResultValue);
        }

        [Fact(DisplayName = "CMD.FindOne")]
        public void FindOne()
        {
            var models = this.AddMockModels();
            var model = models.RandomOne();

            var command = new CMD.FindOne<Models.Member, Models.Member> { Where = this.Engine.GetRemoveWhere<Models.Member>(model.Id) };
            Assert.Null(GA.Compare(model, this.Execute(command).ResultValue));
        }
        [Fact(DisplayName = "CMD.FindOneWhere")]
        public void FindOneWhere()
        {
            var models = this.AddMockModels();
            var model = models.RandomOne();

            var command = new CMD.FindOne<Models.Member, Models.Member>()
            {
                Where = WhereParameters.Parse(this.Engine, new { model.IdCard, model.Email })
            };
            Assert.Null(GA.Compare(model, this.Execute(command).ResultValue));
        }


        [Fact(DisplayName = "CMD.FindAllWhere")]
        public void FindAllWhere1()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Email = "Email" + (index++ % 2));
            var model = models.RandomOne();

            var command = new CMD.FindAll<Models.Member, Models.Member>()
            {
                Where = WhereParameters.Parse(this.Engine, new { Email = "Email1" })
            };
            Assert.Equal(models.Where(m => m.Email == "Email1").Count(), this.Execute(command).ResultValue.Count);
        }
        [Fact(DisplayName = "CMD.FindAllWhere - View")]
        public void FindAllWhere2()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Email = "Email" + (index++ % 2));
            var model = models.RandomOne();

            var command = new CMD.FindAll<Models.Member, Models.MemberViewModel>()
            {
                Where = WhereParameters.Parse(this.Engine, new { Email = "Email1" })
            };
            Assert.Equal(models.Where(m => m.Email == "Email1").Count(), this.Execute(command).ResultValue.Count);
        }

        [Fact(DisplayName = "CMD.FindAllPage")]
        public void FindAllPage1()
        {
            var index = 0;
            var models = this.AddMockModels(m => m.Email = "Email" + (index++ % 2));
            var model = models.RandomOne();

            var command = new CMD.FindAllPage<Models.Member, Models.Member>()
            {
                Where = WhereParameters.Parse(this.Engine, new { Email = "Email1" }),
                Page = new Pagination(2, 3)
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
                Where = WhereParameters.Parse(this.Engine, new { Email = "Email1" }),
                Page = new Pagination(2, 3)
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
                Where = WhereParameters.Parse(this.Engine, new Data.ExecuteParameterCollection(DbExtensions.GetKeyValues<Models.Member>(null, model.Id)))
            };
            Assert.True(this.Execute(command).ResultValue);

            command.Where = WhereParameters.Parse(this.Engine, new Data.ExecuteParameterCollection(DbExtensions.GetKeyValues<Models.Member>("Username", model.Username)));
            Assert.True(this.Execute(command).ResultValue);

            command.Where = WhereParameters.Parse(this.Engine, new Data.ExecuteParameterCollection(DbExtensions.GetKeyValues<Models.Member>("Username", "A$$$$$$$$$$")));
            Assert.False(this.Execute(command).ResultValue);
        }

        [Fact(DisplayName = "CMD.ExistsWhere")]
        public void ExistsWhere()
        {
            var models = this.AddMockModels();
            var model = models.RandomOne();

            var command = new CMD.Exists<Models.Member>()
            {
                Where = WhereParameters.Parse(this.Engine, new { model.Username }),
            };
            Assert.True(this.Execute(command).ResultValue);

            command.Where = WhereParameters.Parse(this.Engine, new { Username = "A$$$$$$$$$$" });
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
                Where = WhereParameters.Parse(this.Engine, new { model.Username }),
            };
            Assert.Equal(1, this.Execute(command).ResultValue);

            var query = models.Where(m => m.Email == "Email1");
            command.Where = WhereParameters.Parse(this.Engine, new { Email = "Email1" });
            Assert.Equal(query.Count(), this.Execute(command).ResultValue);
        }
    }
}
