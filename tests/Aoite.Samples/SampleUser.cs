using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Samples
{
    class SampleUser
    {
        [Aoite.Data.Column(true)]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
