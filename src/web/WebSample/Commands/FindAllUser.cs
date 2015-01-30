using Aoite.CommandModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSample.Arguments;
using WebSample.Models;

namespace WebSample.Commands
{
    public class FindAllUser : UserFindAllArguments, ICommand<GridData<User>>
    {
        public GridData<User> ResultValue { get; set; }
    }
    class FindAllUserExecutor : IExecutor<FindAllUser>
    {
        void IExecutor<FindAllUser>.Execute(IContext context, FindAllUser command)
        {
            var builder = context.Engine.Select<User>();
            if(!string.IsNullOrEmpty(command.LikeUsername))
                builder.Where("Username LIKE @username", "@username", command.LikeUsername.ToLiking());
            if(!string.IsNullOrEmpty(command.LikeRealName))
                builder.Where("RealName LIKE @realName", "@realName", command.LikeRealName.ToLiking());

            command.ResultValue = builder.ToEntities<User>(command).UnsafeValue;
        }
    }
}