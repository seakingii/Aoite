using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSample.Models
{
    public class User
    {
        [Column(true)]
        public Guid Id { get; set; }
        public string RealName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}