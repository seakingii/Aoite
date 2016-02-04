using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoite.Data.Factories
{
    /// <summary>
    /// 表示一个基于 Microsoft SQL Server 数据源命令生成工厂。
    /// </summary>
    public class SqlFactory : ISqlFactory
    {
        /// <summary>
        /// 获取数据库命令生成工厂的唯一实例。
        /// </summary>
        public readonly static SqlFactory Instance = new SqlFactory();
        /// <summary>
        /// 获取一个值，表示空的个性化暗道。
        /// </summary>
        public readonly static ICommandTunnel Empty = new EmptyCommandTunnel();

        /// <summary>
        /// 初始化一个 <see cref="SqlFactory"/> 类的新实例。
        /// </summary>
        protected SqlFactory() { }

        /// <summary>
        /// 转义指定位置的名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="point">名称的位置。</param>
        /// <returns>转义后的名称。</returns>
        public virtual string EscapeName(string name, NamePoint point)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            switch(point)
            {
                case NamePoint.Field:
                case NamePoint.Table:
                    if(name[0] == '[') return name;
                    return string.Concat("[", name, "]");
                case NamePoint.Value:
                case NamePoint.Parameter:
                    if(name[0] == '@') return name;
                    return string.Concat("@", name);
            }
            return name;
        }

        /// <summary>
        /// 反转义名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>反转义后的名称。</returns>
        public virtual string UnescapeName(string name)
        {
            if(!string.IsNullOrWhiteSpace(name))
            {
                if(name.Length > 1 && name[0] == '[') return name.Substring(1, name.Length - 2);
                if(name[0] == '@') return name.Substring(1);
            }
            return name;
        }

        /// <summary>
        /// 获取指定类型映射器的属性集合。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">类型映射器关联的对象实例，并将 <paramref name="entity"/> 转换成映射器的类型 。</param>
        /// <returns>映射属性的集合枚举。</returns>
        protected virtual IEnumerable<PropertyMapper> FindProperties(TypeMapper mapper, ref object entity)
        {
            IEnumerable<PropertyMapper> pms = mapper.Properties;
            var entityType = entity.GetType();
            if(entityType.IsAssignableFrom(mapper.Type)) return pms;

            var mapper2 = TypeMapper.Create(entityType);

            var query = from op in pms
                        join np in mapper2.Properties on op.Property.Name.ToLower() equals np.Name.ToLower()
                        select new { op, np };
            var entity2 = Activator.CreateInstance(mapper.Type, true);

            List<PropertyMapper> pms2 = new List<PropertyMapper>();
            foreach(var item in query)
            {
                pms2.Add(item.op);
                var value = item.np.GetValue(entity, false);
                if(!item.op.Property.PropertyType.IsAssignableFrom(item.np.Property.PropertyType))
                {
                    value = Convert.ChangeType(value, item.op.Property.PropertyType);
                }

                item.op.SetValue(entity2, value, false);
            }
            entity = entity2;
            return pms2;
        }

        /// <summary>
        /// 追加指定属性映射器名称到 <paramref name="builder"/>，并将其参数和值添加到集合。
        /// </summary>
        /// <param name="property">属性映射器。</param>
        /// <param name="builder">字符串生成器。</param>
        /// <param name="value">属性的值。</param>
        /// <param name="ps">参数集合。</param>
        protected virtual void AppendParameterValue(PropertyMapper property, StringBuilder builder, object value, ExecuteParameterCollection ps)
        {
            var upperName = property.Name.ToUpper();
            builder.Append(this.EscapeName(upperName, NamePoint.Value));
            ps.Add(this.EscapeName(upperName, NamePoint.Parameter), value);
        }

        /// <summary>
        /// 指定类型映射器创建一个获取最后递增序列值的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>查询命令。</returns>
        public virtual ExecuteCommand CreateLastIdentityCommand(TypeMapper mapper, ICommandTunnel tunnel = null)
        {
            if(mapper == null) throw new ArgumentNullException(nameof(mapper));
            if(tunnel == null) tunnel = Empty;

            return tunnel.GetCommand(mapper, new ExecuteCommand("SELECT @@IDENTITY"));
        }

        /// <summary>
        /// 指定类型映射器和实体创建一个插入的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>查询命令。</returns>
        public virtual ExecuteCommand CreateInsertCommand(TypeMapper mapper, object entity, ICommandTunnel tunnel = null)
        {
            if(mapper == null) throw new ArgumentNullException(nameof(mapper));
            if(entity == null) throw new ArgumentNullException(nameof(entity));
            if(tunnel == null) tunnel = Empty;

            if(mapper.Count == 0) throw new NotSupportedException($"{entity.GetType().FullName} 的插入操作没有找到任何属性。");

            var fieldsBuilder = new StringBuilder("INSERT INTO ")
                                .Append(this.EscapeName(tunnel.GetTableName(mapper), NamePoint.Table))
                                .Append('(');
            var valueBuilder = new StringBuilder(")VALUES(");
            var ps = new ExecuteParameterCollection(mapper.Count);
            int index = 0;
            foreach(var property in FindProperties(mapper, ref entity))
            {
                if(property.IsIgnore) continue;
                var value = property.GetValue(entity, true);
                if(property.IsKey && object.Equals(value, property.TypeDefaultValue)) continue;

                if(index++ > 0)
                {
                    fieldsBuilder.Append(',');
                    valueBuilder.Append(',');
                }
                fieldsBuilder.Append(this.EscapeName(property.Name, NamePoint.Field));
                this.AppendParameterValue(property, valueBuilder, value, ps);
            }
            return tunnel.GetCommand(mapper, new ExecuteCommand(fieldsBuilder.Append(valueBuilder.Append(')').ToString()).ToString(), ps));
        }

        /// <summary>
        /// 指定类型映射器和实体创建一个更新的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>查询命令。</returns>
        public virtual ExecuteCommand CreateUpdateCommand(TypeMapper mapper, object entity, WhereParameters where, ICommandTunnel tunnel = null)
        {
            if(mapper == null) throw new ArgumentNullException(nameof(mapper));
            if(entity == null) throw new ArgumentNullException(nameof(entity));
            if(where == null) throw new ArgumentNullException(nameof(where));
            if(tunnel == null) tunnel = Empty;

            var setBuilder = new StringBuilder("UPDATE ")
                                .Append(this.EscapeName(tunnel.GetTableName(mapper), NamePoint.Table))
                                .Append(" SET ");
            var ps = where.Parameters ?? new ExecuteParameterCollection(mapper.Count);

            int index = 0;
            foreach(var property in FindProperties(mapper, ref entity))
            {
                if(property.IsIgnore || property.IsKey) continue;

                if(index++ > 0) setBuilder.Append(',');

                setBuilder.Append(this.EscapeName(property.Name, NamePoint.Field))
                          .Append('=');
                var value = property.GetValue(entity, true);
                this.AppendParameterValue(property, setBuilder, value, ps);
            }
            if(index == 0) throw new NotSupportedException($"{entity.GetType().FullName} 的更新操作没有找到任何属性。");

            return tunnel.GetCommand(mapper, new ExecuteCommand(where.AppendTo(setBuilder.ToString()), ps));
        }

        /// <summary>
        /// 指定类型映射器和条件参数创建一个删除的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>查询命令。</returns>
        public virtual ExecuteCommand CreateDeleteCommand(TypeMapper mapper, WhereParameters where, ICommandTunnel tunnel = null)
        {
            if(mapper == null) throw new ArgumentNullException(nameof(mapper));
            if(where == null) throw new ArgumentNullException(nameof(where));
            if(tunnel == null) tunnel = Empty;

            var commandText = "DELETE FROM " + this.EscapeName(tunnel.GetTableName(mapper), NamePoint.Table);
            return tunnel.GetCommand(mapper, new ExecuteCommand(where.AppendTo(commandText), where.Parameters));
        }

        /// <summary>
        /// 创建指定视图类型的字段列表。
        /// </summary>
        /// <param name="entityMapper">实体的类型映射器。</param>
        /// <param name="viewMapper">视图的类型映射器。</param>
        /// <returns>包含在 <paramref name="entityMapper"/> 的 <paramref name="viewMapper"/> 属性集合，并对每个属性进行转义。</returns>
        public virtual string CreateFields(TypeMapper entityMapper, TypeMapper viewMapper)
        {
            if(entityMapper == null) throw new ArgumentNullException(nameof(entityMapper));
            if(viewMapper == null) throw new ArgumentNullException(nameof(viewMapper));

            string fields = "*";
            if(entityMapper != viewMapper)
            {
                List<string> fieldList = new List<string>();
                foreach(var viewProperty in viewMapper.Properties)
                {
                    if(viewProperty.IsIgnore) continue;
                    if(!entityMapper.Contains(viewProperty.Name)) continue;
                    fieldList.Add(this.EscapeName(viewProperty.Name, NamePoint.Field));
                }
                fields = fieldList.ToArray().Join();
            }
            return fields;
        }

        /// <summary>
        /// 指定实体类型映射器、视图映射器和条件创建一个查询的命令。
        /// </summary>
        /// <param name="entityMapper">实体的类型映射器。</param>
        /// <param name="viewMapper">视图的类型映射器。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="top">指定 TOP 数量，小于 1 则忽略作用。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>查询命令。</returns>
        public virtual ExecuteCommand CreateQueryCommand(TypeMapper entityMapper, TypeMapper viewMapper, WhereParameters where, int top = 0, ICommandTunnel tunnel = null)
        {
            if(entityMapper == null) throw new ArgumentNullException(nameof(entityMapper));
            if(viewMapper == null) throw new ArgumentNullException(nameof(viewMapper));

            var fields = this.CreateFields(entityMapper, viewMapper);
            return CreateQueryCommand(entityMapper, fields, where, top, tunnel);
        }

        /// <summary>
        /// 指定实体类型映射器、列集合和条件创建一个查询的命令。
        /// </summary>
        /// <param name="entityMapper">实体的类型映射器。</param>
        /// <param name="fields">SELECT 的列集合(以“,”分隔），可以为 null 值，表示所有字段。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="top">指定 TOP 数量，小于 1 则忽略作用。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>查询命令。</returns>
        public virtual ExecuteCommand CreateQueryCommand(TypeMapper entityMapper, string fields, WhereParameters where, int top = 0, ICommandTunnel tunnel = null)
        {
            if(entityMapper == null) throw new ArgumentNullException(nameof(entityMapper));
            if(where == null) throw new ArgumentNullException(nameof(where));

            if(tunnel == null) tunnel = Empty;
            if(string.IsNullOrWhiteSpace(fields)) fields = "*";
            if(top > 0) fields = string.Concat("TOP ", top.ToString(), " ", fields);

            var commandText = string.Concat("SELECT ", fields, " FROM ", this.EscapeName(tunnel.GetTableName(entityMapper), NamePoint.Table));
            return tunnel.GetCommand(entityMapper, new ExecuteCommand(where.AppendTo(commandText), where.Parameters));
        }

        /// <summary>
        /// 指定类型映射器和条件参数创建一个行是否存在的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>查询命令。</returns>
        public virtual ExecuteCommand CreateExistsCommand(TypeMapper mapper, WhereParameters where, ICommandTunnel tunnel = null)
        {
            if(mapper == null) throw new ArgumentNullException(nameof(mapper));
            if(where == null) throw new ArgumentNullException(nameof(where));
            if(tunnel == null) tunnel = Empty;

            var commandText = "SELECT 1 FROM " + this.EscapeName(tunnel.GetTableName(mapper), NamePoint.Table);
            return tunnel.GetCommand(mapper, new ExecuteCommand(where.AppendTo(commandText), where.Parameters));
        }

        /// <summary>
        /// 指定类型映射器和条件参数创建一个表总行数的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="where">条件参数。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>查询命令。</returns>
        public virtual ExecuteCommand CreateRowCountCommand(TypeMapper mapper, WhereParameters where, ICommandTunnel tunnel = null)
        {
            if(mapper == null) throw new ArgumentNullException(nameof(mapper));
            if(where == null) throw new ArgumentNullException(nameof(where));
            if(tunnel == null) tunnel = Empty;

            var commandText = "SELECT COUNT(*) FROM " + this.EscapeName(tunnel.GetTableName(mapper), NamePoint.Table);
            return tunnel.GetCommand(mapper, new ExecuteCommand(where.AppendTo(commandText), where.Parameters));
        }

        #region PageProvider

        /// <summary>
        /// 获取分页的字符串格式项。
        /// </summary>
        protected virtual string PageFormat
        {
            get
            {
                return @"SELECT * FROM (SELECT ROW_NUMBER() OVER({4}) AS {1},* FROM ({0}) ____t1____) ____t2____ WHERE {1}>{2} AND {1}<={3}";
            }
        }

        /// <summary>
        /// 获取统计的字符串格式项。
        /// </summary>
        protected virtual string TotalFormat
        {
            get { return @"SELECT COUNT(*) FROM ({0}) ____t____"; }
        }

        static readonly Regex OrderByRegex = new Regex(@"\s*order\s+by\s+[^\s,\)\(]+(?:\s+(?:asc|desc))?(?:\s*,\s*[^\s,\)\(]+(?:\s+(?:asc|desc))?)*", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// 获取默认页码字段的列名。
        /// </summary>
        public const string DefaultRowNumberName = "_RN_";

        /// <summary>
        /// 获取最后一个匹配的 Order By 结果。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        /// <returns> Order By 结果。</returns>
        protected static Match GetOrderByMatch(string commandText)
        {
            var match = OrderByRegex.Match(commandText);
            while(match.Success)
            {
                if((match.Index + match.Length) == commandText.Length) return match;
                match = match.NextMatch();
            }
            return match;
        }

        /// <summary>
        /// 创建指定查询字符串的统计总行数查询字符串。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        public virtual string CreatePageTotalCountCommand(string commandText)
        {
            var match = GetOrderByMatch(commandText);
            return string.Format(TotalFormat, match.Success ? commandText.Remove(match.Index) : commandText);
        }

        /// <summary>
        /// 对指定的 <see cref="DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        public virtual void PageProcessCommand(int pageNumber, int pageSize, DbCommand command)
        {
            var start = (pageNumber - 1) * pageSize;
            var end = pageNumber * pageSize;
            var match = GetOrderByMatch(command.CommandText);
            var orderBy = "ORDER BY getdate()";
            if(match.Success)
            {
                command.CommandText = command.CommandText.Remove(match.Index);
                orderBy = match.Value.Trim();
            }

            command.CommandText = string.Format(PageFormat
                , command.CommandText
                , DefaultRowNumberName
                , start
                , end
                , orderBy);
        }

        #endregion
    }
}
