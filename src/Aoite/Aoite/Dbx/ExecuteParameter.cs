using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Dbx
{
    class ExecuteParameter
    {
        public ExecuteParameter()
        {
            int id = 5;
            var name = "a";
            var s = $@"select * from user where id={id:int}
and username ={name:varchar(50)}";

            var s2 = "@name varchar(50)";
        }
    }
}
