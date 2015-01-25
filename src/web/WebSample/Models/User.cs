using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSample.Models
{
    [Serializable]
    public class User
    {
        public string Username { get; set; }
        public DateTime LoginTime { get; set; }
    }
}