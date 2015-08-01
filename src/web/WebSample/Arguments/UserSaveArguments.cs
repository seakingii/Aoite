using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSample.Arguments
{
    public class UserSaveArguments
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "姓名不能为空。")]
        public string RealName { get; set; }
        [Required(ErrorMessage = "账号不能为空。")]
        public string Username { get; set; }
        [Required(ErrorMessage = "密码不能为空。")]
        public string Password { get; set; }
    }
}