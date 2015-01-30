using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSample.ViewModels
{
    public class LoggedUser
    {
        public Guid Id { get; set; }
        public string RealName { get; set; }
        public string Username { get; set; }
        [Ignore]
        public DateTime LoginTime { get; set; }
    }
}