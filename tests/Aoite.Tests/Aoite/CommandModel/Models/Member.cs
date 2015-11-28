using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.CommandModel.Models
{
    public enum MemberSex
    {
        Unkonw,
        Male,
        Female
    }
    public enum MemberRole
    {
        Taker,
        Publisher,
    }
    public enum MemberStatus
    {
        Normal,
        Disabled
    }
    public class Member
    {
        [Aoite.Data.Column(true)]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AdvPassword { get; set; }
        public MemberRole RoleType { get; set; }
        public int VipLevel { get; set; }
        public long Score { get; set; }
        public long Money { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string RealName { get; set; }
        public MemberSex Sex { get; set; }
        public string IdCard { get; set; }
        public string IdCardPic { get; set; }
        public bool IsRealAuth { get; set; }

        public DateTime? RealAuthTime { get; set; }
        public DateTime RegisterTime { get; set; }

        public long CommHaoNum { get; set; }
        public long CommZhongNum { get; set; }
        public long CommChaNum { get; set; }

        public long TaskPublishNum { get; set; }
        public long TaskFaildNum { get; set; }
        public long TaskSuccessNum { get; set; }
        public long TaskFailNum { get; set; }

        public MemberStatus Status { get; set; }

    }

    public class MemberViewModel
    {
        [Aoite.Data.Column(true)]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
