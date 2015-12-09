using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源查询与交互引擎的提供程序基类。
    /// </summary>
    public abstract class DbEngineProviderBase : IDbEngineProvider
    {
        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        public abstract DbProviderFactory DbFactory { get; }

        /// <summary>
        /// 获取一个值，表示当前数据提供程序的名称。
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 获取用于当前数据源的连接字符串。
        /// </summary>
        public virtual string ConnectionString { get; }

        /// <summary>
        /// 指定数据库的连接字符串，初始化一个 <see cref="DbEngineProviderBase"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">数据源的连接字符串。</param>
        public DbEngineProviderBase(string connectionString)
        {
            if(string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// 转义指定位置的名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="point">名称的位置。</param>
        /// <returns>转义后的名称。</returns>
        public virtual string EscapeName(string name, NamePoint point)
        {
            switch(point)
            {
                case NamePoint.Field:
                case NamePoint.Table:
                    return string.Concat("[", name, "]");
                case NamePoint.Parameter:
                case NamePoint.Declare:
                    return string.Concat("@", name);
            }
            return name;
        }

        /// <summary>
        /// 创建并返回一个到数据源的连接。
        /// </summary>
        /// <returns>一个到数据源的连接。</returns>
        public virtual DbConnection CreateConnection()
        {
            var conn = this.DbFactory.CreateConnection();
            conn.ConnectionString = this.ConnectionString;
            return conn;
        }

        /// <summary>
        /// 指定类型映射器创建一个获取最后递增序列值的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <returns>一个查询命令。</returns>
        public virtual ExecuteCommand CreateLastIdentityCommand(TypeMapper mapper)
        {
            return new ExecuteCommand("SELECT @@IDENTITY");
        }

        /// <summary>
        /// 指定类型映射器和实体创建一个插入的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        public virtual ExecuteCommand CreateInsertCommand(TypeMapper mapper, object entity, string tableName = null)
        {
            if(mapper == null) throw new ArgumentNullException(nameof(mapper));
            if(entity == null) throw new ArgumentNullException(nameof(entity));
            if(mapper.Count == 0) throw new NotSupportedException("{0} 的插入操作没有找到任何属性。".Fmt(entity.GetType().FullName));

            var fieldsBuilder = new StringBuilder("INSERT INTO ")
                                .Append(this.EscapeName(tableName ?? mapper.Name, NamePoint.Table))
                                .Append('(');
            var valueBuilder = new StringBuilder(")VALUES(");
            var ps = new ExecuteParameterCollection(mapper.Count);
            int index = 0;
            foreach(var property in FindProperties(mapper, ref entity))
            {
                if(property.IsIgnore) continue;
                var value = property.GetValue(entity);
                if(property.IsKey && object.Equals(value, property.TypeDefaultValue)) continue;

                if(index++ > 0)
                {
                    fieldsBuilder.Append(',');
                    valueBuilder.Append(',');
                }
                fieldsBuilder.Append(this.EscapeName(property.Name, NamePoint.Field));
                DefaultAppendValue(property, valueBuilder, value, ps);
            }
            return new ExecuteCommand(fieldsBuilder.Append(valueBuilder.Append(')').ToString()).ToString(), ps);
        }

        /// <summary>
        /// 指定类型映射器和实体创建一个更新的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        public virtual ExecuteCommand CreateUpdateCommand(TypeMapper mapper, object entity, string tableName = null)
        {
            if(mapper == null) throw new ArgumentNullException(nameof(mapper));
            if(entity == null) throw new ArgumentNullException(nameof(entity));

            var setBuilder = new StringBuilder("UPDATE ")
                                .Append(this.EscapeName(tableName ?? mapper.Name, NamePoint.Table))
                                .Append(" SET ");
            var whereBuilder = new StringBuilder();
            var ps = new ExecuteParameterCollection(mapper.Count);

            int index = 0;
            foreach(var property in FindProperties(mapper, ref entity))
            {
                if(property.IsIgnore) continue;

                StringBuilder builder;
                if(property.IsKey)
                {
                    builder = whereBuilder;
                    if(builder.Length > 0) builder.Append(" AND ");
                }
                else
                {
                    builder = setBuilder;
                    if(index++ > 0) builder.Append(',');
                }

                builder.Append(this.EscapeName(property.Name, NamePoint.Field));
                builder.Append('=');
                var value = property.GetValue(entity);
                DefaultAppendValue(property, builder, value, ps);
            }

            if(whereBuilder.Length == 0) throw new NotSupportedException("{0} 的更新操作没有找到主键。".Fmt(entity.GetType().FullName));
            setBuilder.Append(" WHERE ").Append(whereBuilder.ToString());
            return new ExecuteCommand(setBuilder.ToString(), ps);
        }

        /// <summary>
        /// 指定类型映射器和实体创建一个删除的命令。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entityOrPKValue">实体的实例对象（引用类型）或一个主键的值（值类型）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询命令。</returns>
        public virtual ExecuteCommand CreateDeleteCommand(TypeMapper mapper, object entityOrPKValue, string tableName = null)
        {
            if(mapper == null) throw new ArgumentNullException(nameof(mapper));
            if(entityOrPKValue == null) throw new ArgumentNullException(nameof(entityOrPKValue));

            var type = entityOrPKValue.GetType();
            if(type.IsSimpleType()) return CreateDeleteCommandWithPK(mapper, entityOrPKValue, tableName);

            if(entityOrPKValue is Array) return CreateDeleteCommandWithPK(mapper, entityOrPKValue, tableName);

            if(entityOrPKValue is System.Collections.IEnumerable)
            {
                List<object> items = new List<object>();
                foreach(var item in (System.Collections.IEnumerable)entityOrPKValue) items.Add(item);
                return CreateDeleteCommand(mapper, items.ToArray(), tableName);
            }
            return CreateDeleteCommandWithEntity(mapper, entityOrPKValue, tableName);
        }

        private ExecuteCommand CreateDeleteCommandWithPK(TypeMapper mapper, object value, string tableName)
        {
            var whereBuilder = new StringBuilder();
            var ps = new ExecuteParameterCollection(mapper.Count);

            foreach(var property in mapper.Properties)
            {
                if(property.IsKey)
                {
                    var arrayValue = value as Array;
                    var isArrayValue = arrayValue != null;
                    int index = 0;
                    var fName = property.Name.ToUpper();
                    ARRAY_LABEL:
                    var pName = fName;
                    if(isArrayValue) pName += index;

                    whereBuilder.Append(this.EscapeName(fName, NamePoint.Field))
                                .Append('=')
                                .Append(this.EscapeName(pName, NamePoint.Parameter));
                    if(isArrayValue)
                    {
                        ps.Add(this.EscapeName(pName, NamePoint.Declare), arrayValue.GetValue(index++));
                        if(index < arrayValue.Length)
                        {
                            whereBuilder.Append(" OR ");
                            goto ARRAY_LABEL;
                        }
                    }
                    else ps.Add(this.EscapeName(pName, NamePoint.Declare), value);
                    break;
                }
            }
            return this.CreateDeleteCommand(mapper, whereBuilder, ps, tableName);
        }

        private ExecuteCommand CreateDeleteCommandWithEntity(TypeMapper mapper, object entity, string tableName)
        {
            var whereBuilder = new StringBuilder();
            var ps = new ExecuteParameterCollection(mapper.Count);

            int index = 0;
            foreach(var property in FindProperties(mapper, ref entity))
            {
                if(property.IsKey)
                {
                    if(whereBuilder.Length > 0) whereBuilder.Append(" AND ");

                    if(index++ > 0) whereBuilder.Append(',');

                    whereBuilder.Append(this.EscapeName(property.Name, NamePoint.Field));
                    whereBuilder.Append('=');
                    var value = property.GetValue(entity);
                    DefaultAppendValue(property, whereBuilder, value, ps);
                }
            }
            return this.CreateDeleteCommand(mapper, whereBuilder, ps, tableName);
        }

        private ExecuteCommand CreateDeleteCommand(TypeMapper mapper, StringBuilder whereBuilder, ExecuteParameterCollection ps, string tableName)
        {
            if(whereBuilder.Length == 0) throw new NotSupportedException("{0} 的删除操作没有找到主键。".Fmt(mapper.Type.FullName));

            whereBuilder.Insert(0, " WHERE ");
            whereBuilder.Insert(0, this.EscapeName(tableName ?? mapper.Name, NamePoint.Table));
            whereBuilder.Insert(0, "DELETE ");

            return new ExecuteCommand(whereBuilder.ToString(), ps);
        }

        private IEnumerable<PropertyMapper> FindProperties(TypeMapper mapper, ref object entity)
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
                var value = item.np.GetValue(entity);
                if(!item.op.Property.PropertyType.IsAssignableFrom(item.np.Property.PropertyType))
                {
                    value = Convert.ChangeType(value, item.op.Property.PropertyType);
                }

                item.op.SetValue(entity2, value);
            }
            entity = entity2;
            return pms2;
        }

        private void DefaultAppendValue(PropertyMapper property, StringBuilder builder, object value, ExecuteParameterCollection ps)
        {
            var upperName = property.Name.ToUpper();
            builder.Append(this.EscapeName(upperName, NamePoint.Parameter));
            ps.Add(this.EscapeName(upperName, NamePoint.Declare), value);
        }

        /// <summary>
        /// 创建指定查询字符串的统计总行数查询字符串。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        public abstract string CreatePageTotalCountCommand(string commandText);

        /// <summary>
        /// 对指定的 <see cref="DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        public abstract void PageProcessCommand(int pageNumber, int pageSize, DbCommand command);
    }
}
