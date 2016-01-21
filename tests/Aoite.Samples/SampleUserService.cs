using Aoite.CommandModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Samples
{
    class SampleUserService : CommandModelServiceBase
    {
        public bool Add(string username, string password)
        {
            if(string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if(string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            if(this.User != "admin") return false;

            if(Bus.Exists<SampleUser>("Username", username)) return false;

            return Bus.AddAnonymous<SampleUser>(new { username, password }) > 0;
        }

        public bool Check(string username, string password)
        {
            if(string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if(string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            return Bus.Filter(new { username, password }).Exists<SampleUser>();
        }

        public bool Remove(string username)
        {
            if(string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if(this.User != "admin") return false;
            return Bus.Filter(new { username }).Remove<SampleUser>() > 0;
        }

        public long Count()
        {
            return this.Execute(new SampleUserCountCommand()).Result;
        }

        public bool ModifyPassowrd(string newPassword)
        {
            if(this.User == null) return false;
            if(newPassword == null) throw new ArgumentNullException(nameof(newPassword));

            return Bus.Filter(new { username = this.User }).Modify<SampleUser>(new { password = newPassword }) > 0;
        }

        public List<SampleUser> GetList()
        {
            return Bus.Filter().FindAll<SampleUser>();
        }
    }


    [Cache("SAMPLEUSER")]
    class SampleUserCountCommand : CommandBase<long>, ICommandCache
    {
        public ICommandCacheStrategy CreateStrategy(IContext context)
        {
            return new CommandCacheStrategy("COUNT", TimeSpan.FromSeconds(10), this, context);
        }

        public object GetCacheValue()
        {
            return this.Result;
        }

        public bool SetCacheValue(object value)
        {
            if(value is long)
            {
                this.Result = (long)value;
                return true;
            }
            return false;
        }

        class Executor : IExecutor<SampleUserCountCommand>
        {
            public void Execute(IContext context, SampleUserCountCommand command)
            {
                command.Result = context.Engine.RowCount<SampleUser>();
            }
        }
    }
}
