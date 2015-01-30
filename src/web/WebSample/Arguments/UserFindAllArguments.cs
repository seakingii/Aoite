using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSample.Arguments
{
    public class UserFindAllArguments : PgParameters
    {
        public string LikeUsername { get; set; }
        public string LikeRealName { get; set; }
    }
}