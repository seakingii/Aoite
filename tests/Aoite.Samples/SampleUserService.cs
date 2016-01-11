using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Samples
{
    class SampleUserService : CommandModel.CommandModelServiceBase
    {

        public long Add(string username, string password)
        {
            if(string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if(string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            return this.Execute(new CMD.Add<SampleUser>(true)
            {
                Entity = new SampleUser() { Username = User, Password = password }
            }).ResultValue;
        }
    }
}
