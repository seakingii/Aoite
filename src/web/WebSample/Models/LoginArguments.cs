using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSample.Models
{
    public class LoginArguments
    {
        [Display(Name = "账号"), Required, StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }
        [Display(Name = "密码"), Required, StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }
    }
}