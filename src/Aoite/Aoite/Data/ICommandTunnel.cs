using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个命令的暗道，用于处理命令生成后的处理和个性化表名的接口。
    /// </summary>
    public interface ICommandTunnel
    {
        /// <summary>
        /// 获取个性化表名。
        /// </summary>
        /// <param name="mapper">映射器。</param>
        /// <returns>字符串。</returns>
        string GetTableName(TypeMapper mapper);
        /// <summary>
        /// 给定映射器和执行命令，获取一个个性化的执行命令。
        /// </summary>
        /// <param name="mapper">映射器。</param>
        /// <param name="command">执行命令。</param>
        /// <returns>新的执行命令。</returns>
        ExecuteCommand GetCommand(TypeMapper mapper, ExecuteCommand command);
    }
    class EmptyCommandTunnel : ICommandTunnel
    {
        public EmptyCommandTunnel() { }
        public ExecuteCommand GetCommand(TypeMapper mapper, ExecuteCommand command)
        {
            return command;
        }

        public string GetTableName(TypeMapper mapper)
        {
            return mapper.Name;
        }
    }
}
