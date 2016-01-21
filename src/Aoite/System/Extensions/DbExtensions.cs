using Aoite.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 提供数据源操作的实用工具方法。
    /// </summary>
    public static partial class DbExtensions
    {
        /// <summary>
        /// 数据源表主键的默认字段名。
        /// </summary>
        public const string DefaultKeyName = "Id";

        #region Ado

        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        public static void AddParameter(this DbCommand dbCommand, string name, object value)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var p = dbCommand.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            dbCommand.Parameters.Add(p);
        }

        /// <summary>
        /// 获取指定参数索引的值。
        /// </summary>
        /// <typeparam name="T">值的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="index">参数索引。</param>
        /// <returns>强类型的值。</returns>
        public static T GetValue<T>(this DbCommand dbCommand, int index)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));
            return dbCommand.Parameters[index].Value.CastTo<T>();
        }

        /// <summary>
        /// 获取指定参数名称的值。
        /// </summary>
        /// <typeparam name="T">值的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="name">参数名称。</param>
        /// <returns>强类型的值。</returns>
        public static T GetValue<T>(this DbCommand dbCommand, string name)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            return dbCommand.Parameters[name].Value.CastTo<T>();
        }

        /// <summary>
        /// 将指定的执行命令转换成完整字符串形式。
        /// </summary>
        /// <param name="command">执行命令。</param>
        /// <returns>一个完整执行命令的字符串形式。</returns>
        public static string ToFullString(this ExecuteCommand command)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));
            StringBuilder builder = new StringBuilder(command.Text);
            if(command.Parameters != null)
            {
                builder.AppendLine();
                builder.AppendLine("*** Parameters ***");
                foreach(var item in command.Parameters)
                {
                    builder.AppendFormat("{0}\t=\t{1}\r\n", item.Name + (item.Value == null ? string.Empty : " <" + item.Value.GetType().Name + ">"), (item.Value == null || Convert.IsDBNull(item.Value)) ? "<NULL>" : item.Value);
                }
                builder.AppendLine("*** Parameters ***");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 执行查询，转换并返回结果换后的实体。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <returns>一个实体。</returns>
        public static TEntity ExecuteEntity<TEntity>(this DbCommand dbCommand)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));

            var mapper = TypeMapper.Instance<TEntity>.Mapper;
            using(var reader = dbCommand.ExecuteReader())
            {
                if(!reader.Read()) return default(TEntity);
                var value = (TEntity)Activator.CreateInstance(typeof(TEntity), true);
                return mapper.From(reader).To(value);
            }
        }

        /// <summary>
        /// 执行查询，转换并返回结果换后的实体。
        /// </summary>
        /// <param name="dbCommand">数据源命令。</param>
        /// <returns>一个实体。</returns>
        public static dynamic ExecuteEntity(this DbCommand dbCommand)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));

            using(var reader = dbCommand.ExecuteReader())
            {
                if(!reader.Read()) return null;
                return new DynamicEntityValue(reader);
            }
        }

        /// <summary>
        /// 执行查询，转换并返回结果集转换后的实体集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <returns>一个实体集合。</returns>
        public static List<TEntity> ExecuteEntities<TEntity>(this DbCommand dbCommand)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));

            var mapper = TypeMapper.Instance<TEntity>.Mapper;
            using(var reader = dbCommand.ExecuteReader())
            {
                List<TEntity> value = new List<TEntity>();
                while(reader.Read())
                {
                    var model = (TEntity)Activator.CreateInstance(typeof(TEntity), true);
                    mapper.From(reader).To(model);
                    value.Add(model);
                }
                return value;
            }
        }

        /// <summary>
        /// 执行查询，转换并返回结果集转换后的实体集合。
        /// </summary>
        /// <param name="dbCommand">数据源命令。</param>
        /// <returns>一个实体集合</returns>
        public static List<dynamic> ExecuteEntities(this DbCommand dbCommand)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));

            List<dynamic> value = new List<dynamic>();
            using(var reader = dbCommand.ExecuteReader())
            {
                while(reader.Read())
                {
                    value.Add(new DynamicEntityValue(reader));
                }
            }
            return value;
        }

#if !NET40
        /// <summary>
        /// 异步执行查询，转换并返回结果换后的实体。
        /// </summary>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一个实体。</returns>
        public static Task<TEntity> ExecuteEntityAsync<TEntity>(this DbCommand dbCommand, CancellationToken cancellationToken)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));

            return dbCommand.ExecuteReaderAsync(cancellationToken).ContinueWith(t =>
            {
                var mapper = TypeMapper.Instance<TEntity>.Mapper;
                using(var reader = t.Result)
                {
                    if(!reader.Read()) return default(TEntity);
                    var value = (TEntity)Activator.CreateInstance(typeof(TEntity), true);
                    return mapper.From(reader).To(value);
                }
            });
        }

        /// <summary>
        /// 异步执行查询，转换并返回结果换后的实体。
        /// </summary>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一个实体。</returns>
        public static Task<dynamic> ExecuteEntityAsync(this DbCommand dbCommand, CancellationToken cancellationToken)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));

            return dbCommand.ExecuteReaderAsync(cancellationToken).ContinueWith(t =>
            {
                using(var reader = t.Result)
                {
                    if(!reader.Read()) return null;
                    return (dynamic)new DynamicEntityValue(reader);
                }
            });
        }

        /// <summary>
        /// 异步执行查询，转换并返回结果集转换后的实体集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一个实体集合。</returns>
        public static Task<List<TEntity>> ExecuteEntitiesAsync<TEntity>(this DbCommand dbCommand, CancellationToken cancellationToken)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));

            return dbCommand.ExecuteReaderAsync(cancellationToken).ContinueWith(t =>
            {
                var mapper = TypeMapper.Instance<TEntity>.Mapper;
                using(var reader = t.Result)
                {
                    List<TEntity> value = new List<TEntity>();
                    while(reader.Read())
                    {
                        var model = (TEntity)Activator.CreateInstance(typeof(TEntity), true);
                        mapper.From(reader).To(model);
                        value.Add(model);
                    }
                    return value;
                }
            });
        }

        /// <summary>
        /// 异步执行查询，转换并返回结果集转换后的实体集合。
        /// </summary>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <returns>一个实体集合</returns>
        public static Task<List<dynamic>> ExecuteEntitiesAsync(this DbCommand dbCommand, CancellationToken cancellationToken)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));

            return dbCommand.ExecuteReaderAsync(cancellationToken).ContinueWith(t =>
            {
                List<dynamic> value = new List<dynamic>();
                using(var reader = t.Result)
                {
                    while(reader.Read())
                    {
                        value.Add(new DynamicEntityValue(reader));
                    }
                }
                return value;
            });
        }

#endif

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列的强类型值。所有其他的列和行将被忽略。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <returns>结果集中第一行的第一列</returns>
        public static TValue ExecuteScalar<TValue>(this DbCommand dbCommand)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));

            var value = dbCommand.ExecuteScalar();
            if(Convert.IsDBNull(value)) value = default(TValue);
            else if(!(value is TValue)) value = value.CastTo<TValue>();
            return (TValue)value;
        }

        /// <summary>
        /// 执行查询，并返回一张表。
        /// </summary>
        /// <param name="dbCommand">数据库命令。</param>
        /// <param name="dataAdpater">数据源适配器。</param>
        /// <returns>一张表。</returns>
        public static PageTable ExecuteTable(this DbCommand dbCommand, DbDataAdapter dataAdpater)
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));
            if(dataAdpater == null) throw new ArgumentNullException(nameof(dataAdpater));

            PageTable table = new PageTable();
            dataAdpater.Fill(table);
            table.Total = table.Rows.Count;
            return table;

        }

        /// <summary>
        /// 执行查询，并返回一个数据集。
        /// </summary>
        /// <typeparam name="TDataSet">数据集的数据类型。</typeparam>
        /// <param name="dbCommand">数据库命令。</param>
        /// <param name="dataAdpater">数据源适配器。</param>
        /// <returns>一个数据集。</returns>
        public static TDataSet ExecuteDataSet<TDataSet>(this DbCommand dbCommand, DbDataAdapter dataAdpater)
            where TDataSet : DataSet, new()
        {
            if(dbCommand == null) throw new ArgumentNullException(nameof(dbCommand));
            if(dataAdpater == null) throw new ArgumentNullException(nameof(dataAdpater));

            TDataSet dataSet = new TDataSet();
            dataAdpater.Fill(dataSet);
            return dataSet;
        }

        /// <summary>
        /// 当数据源的状态为关闭时，打开连接。
        /// </summary>
        /// <param name="connection">数据库的连接。</param>
        public static void TryOpen(this DbConnection connection)
        {
            if(connection != null && connection.State == ConnectionState.Closed) connection.Open();
        }

        /// <summary>
        /// 当数据源的状态为打开时，尝试关闭连接。该方法可以避免异常的抛出。
        /// </summary>
        /// <param name="connection">数据库的连接。</param>
        public static void TryClose(this DbConnection connection)
        {
            if(connection != null && connection.State != ConnectionState.Closed)
            {
                try
                {
                    connection.Close();
                }
                catch(Exception) { }
            }
        }

        #endregion

        #region Execute

        /// <summary>
        /// 执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="executor">执行器。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一张包含总记录数的表。</returns>
        public static PageTable ToTable(this IDbExecutor executor, IPagination page)
        {
            if(executor == null) throw new ArgumentNullException(nameof(executor));
            if(page == null) throw new ArgumentNullException(nameof(page));
            return executor.ToTable(page.PageNumber, page.PageSize);
        }

#if !NET40
        /// <summary>
        /// 异步执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="executor">执行器。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一张包含总记录数的表。</returns>
        public static Task<PageTable> ToTableAsync(this IDbExecutor executor, IPagination page)
        {
            return executor.ToTableAsync(CancellationToken.None, page);
        }

        /// <summary>
        /// 异步执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="executor">执行器。</param>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一张包含总记录数的表。</returns>
        public static Task<PageTable> ToTableAsync(this IDbExecutor executor, CancellationToken cancellationToken, IPagination page)
        {
            if(executor == null) throw new ArgumentNullException(nameof(executor));
            if(page == null) throw new ArgumentNullException(nameof(page));
            return executor.ToTableAsync(cancellationToken, page.PageNumber, page.PageSize);
        }

        /// <summary>
        /// 异步执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="executor">执行器。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一个包含总记录数的实体集合。</returns>
        public static Task<PageData<TEntity>> ToEntitiesAsync<TEntity>(this IDbExecutor executor, IPagination page)
        {
            return executor.ToEntitiesAsync<TEntity>(CancellationToken.None, page);
        }
        /// <summary>
        /// 异步执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="executor">执行器。</param>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一个包含总记录数的实体集合。</returns>
        public static Task<PageData<TEntity>> ToEntitiesAsync<TEntity>(this IDbExecutor executor, CancellationToken cancellationToken, IPagination page)
        {
            if(executor == null) throw new ArgumentNullException(nameof(executor));
            if(page == null) throw new ArgumentNullException(nameof(page));
            return executor.ToEntitiesAsync<TEntity>(cancellationToken, page.PageNumber, page.PageSize);
        }

        /// <summary>
        /// 执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="executor">执行器。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一个包含总记录数的匿名实体集合。</returns>
        public static Task<PageData<dynamic>> ToEntitiesAsync(this IDbExecutor executor, IPagination page)
        {
            return executor.ToEntitiesAsync(CancellationToken.None, page);
        }
        /// <summary>
        /// 异步执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="executor">执行器。</param>
        /// <param name="cancellationToken">针对取消请求监视的标记。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一个包含总记录数的匿名实体集合。</returns>
        public static Task<PageData<dynamic>> ToEntitiesAsync(this IDbExecutor executor, CancellationToken cancellationToken, IPagination page)
        {
            if(executor == null) throw new ArgumentNullException(nameof(executor));
            if(page == null) throw new ArgumentNullException(nameof(page));
            return executor.ToEntitiesAsync(cancellationToken, page.PageNumber, page.PageSize);
        }

#endif
        /// <summary>
        /// 执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="executor">执行器。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一个包含总记录数的实体集合。</returns>
        public static PageData<TEntity> ToEntities<TEntity>(this IDbExecutor executor, IPagination page)
        {
            if(executor == null) throw new ArgumentNullException(nameof(executor));
            if(page == null) throw new ArgumentNullException(nameof(page));
            return executor.ToEntities<TEntity>(page.PageNumber, page.PageSize);
        }

        /// <summary>
        /// 执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="executor">执行器。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>一个包含总记录数的匿名实体集合。</returns>
        public static PageData<dynamic> ToEntities(this IDbExecutor executor, IPagination page)
        {
            if(executor == null) throw new ArgumentNullException(nameof(executor));
            if(page == null) throw new ArgumentNullException(nameof(page));
            return executor.ToEntities(page.PageNumber, page.PageSize);
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <returns>执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText)
        {
            return engine.Execute(new ExecuteCommand(commandText));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText, ExecuteParameterCollection parameters)
        {
            return engine.Execute(new ExecuteCommand(commandText, parameters));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">匹配 Name/Value 的参数集合或 数组。</param>
        /// <returns>执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText, params object[] parameters)
        {
            return engine.Execute(new ExecuteCommand(commandText, parameters));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText, params ExecuteParameter[] parameters)
        {
            return engine.Execute(new ExecuteCommand(commandText, parameters));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="objectInstance">任意类型的实例。</param>
        /// <returns>执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText, object objectInstance)
        {
            return engine.Execute(new ExecuteCommand(commandText, new ExecuteParameterCollection(objectInstance)));
        }

        #endregion

        #region Adv

        #region Add

        /// <summary>
        /// 执行一个插入的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int Add<TEntity>(this IDbEngine engine, TEntity entity, ICommandTunnel tunnel = null)
            => AddAnonymous<TEntity>(engine, entity, tunnel);

        /// <summary>
        /// 执行一个插入的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int AddAnonymous<TEntity>(this IDbEngine engine, object entity, ICommandTunnel tunnel = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(entity == null) throw new ArgumentNullException(nameof(entity));

            var command = engine.Provider.SqlFactory.CreateInsertCommand(TypeMapper.Instance<TEntity>.Mapper, entity, tunnel);
            return engine.Execute(command).ToNonQuery();
        }

        #endregion

        #region Modify

        /// <summary>
        /// 执行一个更新的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int Modify<TEntity>(this IDbEngine engine, TEntity entity, ICommandTunnel tunnel = null)
            => ModifyAnonymous<TEntity>(engine, entity, tunnel);

        /// <summary>
        /// 执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int ModifyAnonymous<TEntity>(this IDbEngine engine, object entity, ICommandTunnel tunnel = null)
            => Filter(engine, GetModifyKeyValues<TEntity>(entity)).Modify<TEntity>(entity, tunnel);

        #endregion

        #region Remove

        /// <summary>
        /// 执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int Remove<TEntity>(this IDbEngine engine, TEntity entity, ICommandTunnel tunnel = null)
            => RemoveAnonymous<TEntity>(engine, entity, tunnel);

        /// <summary>
        /// 执行一个删除的命令，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entityOrPKValues">实体的实例对象，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>受影响的行。</returns>
        public static int RemoveAnonymous<TEntity>(this IDbEngine engine, object entityOrPKValues, ICommandTunnel tunnel = null)
            => Filter(engine, GetRemoveWhere<TEntity>(engine, entityOrPKValues)).Remove<TEntity>(tunnel);

        #endregion

        #region FineOne

        /// <summary>
        /// 获取指定 <paramref name="keyValue"/> 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        public static TEntity FindOne<TEntity>(this IDbEngine engine, object keyValue, ICommandTunnel tunnel = null)
            => FindOne<TEntity, TEntity>(engine, keyValue, tunnel);

        /// <summary>
        /// 获取指定 <paramref name="keyName"/> 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。可以为 null 值。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        public static TEntity FindOne<TEntity>(this IDbEngine engine, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => FindOne<TEntity, TEntity>(engine, keyName, keyValue, tunnel);

        /// <summary>
        /// 获取指定 <paramref name="keyValue"/> 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        public static TView FindOne<TEntity, TView>(this IDbEngine engine, object keyValue, ICommandTunnel tunnel = null)
            => FindOne<TEntity, TView>(engine, null, keyValue, tunnel);

        /// <summary>
        /// 获取指定 <paramref name="keyName"/> 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。可以为 null 值。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个实体。</returns>
        public static TView FindOne<TEntity, TView>(this IDbEngine engine, string keyName, object keyValue, ICommandTunnel tunnel = null)
            => Filter(engine, GetKeyValues<TEntity>(keyName, keyValue)).FindOne<TEntity, TView>(tunnel);

        #endregion

        #region Exists & RowCount & Select

        /// <summary>
        /// 判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static bool Exists<TEntity>(this IDbEngine engine, object keyValue, ICommandTunnel tunnel = null)
            => Exists<TEntity>(engine, null, keyValue, tunnel);

        /// <summary>
        /// 判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>一个值，表示数据是否存在。</returns>
        public static bool Exists<TEntity>(this IDbEngine engine, string keyName, object keyValue, ICommandTunnel tunnel = null)
             => Filter(engine, GetKeyValues<TEntity>(keyName, keyValue)).Exists<TEntity>(tunnel);

        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>数据的行数。</returns>
        public static long RowCount<TEntity>(this IDbEngine engine, ICommandTunnel tunnel = null)
            => Filter(engine).RowCount<TEntity>(tunnel);

        /// <summary>
        /// 获取最后递增序列值。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="tunnel">用于个性化表名和命令的暗道，可以为 null 值。</param>
        /// <returns>递增序列值。</returns>
        public static long GetLastIdentity<TEntity>(this IDbEngine engine, ICommandTunnel tunnel = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            var command = engine.Provider.SqlFactory.CreateLastIdentityCommand(TypeMapper.Instance<TEntity>.Mapper, tunnel);
            return engine.Execute(command).ToScalar<long>();
        }

        /// <summary>
        /// 添加 SELECT 的字段。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="fields">字段的集合。</param>
        /// <returns> <see cref="ISelect"/> 的实例。</returns>
        public static ISelect Select(this IDbEngine engine, params string[] fields)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            return new SqlBuilder(engine).Select(fields);
        }

        /// <summary>
        /// 添加 SELECT 的字段和 FORM 语句。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="fields">字段的集合。</param>
        /// <returns> <see cref="ISelect"/> 的实例。</returns>
        public static ISelect Select<TEntity>(this IDbEngine engine, params string[] fields)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            return new SqlBuilder(engine).Select(fields).From(engine.Provider.SqlFactory.EscapeName(TypeMapper.Instance<TEntity>.Mapper.Name, NamePoint.Table));
        }

        #endregion

        #endregion

        #region Filter

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this IDbEngine engine)
            => Filter(engine, WhereParameters.Empty);

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this IDbEngine engine, object objectInstance, string binary = "AND")
            => Filter(engine, new ExecuteParameterCollection(objectInstance), binary);

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keysAndValues">应当是 <see cref="string"/> / <see cref="object"/> 的字典集合。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this IDbEngine engine, params object[] keysAndValues)
            => Filter(engine, new ExecuteParameterCollection(keysAndValues));

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this IDbEngine engine, ExecuteParameterCollection ps, string binary = "AND")
            => Filter(engine, CreateWhere(engine, ps, binary), ps);

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="whereCallback">一个创建查询条件的回调方法。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this IDbEngine engine, Action<IWhere> whereCallback)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            var builder = new SqlBuilder(engine);
            whereCallback(builder.Where());
            return Filter(engine, builder.WhereText, builder.Parameters);
        }

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this IDbEngine engine, string where, ExecuteParameterCollection ps)
            => Filter(engine, new WhereParameters(where, ps));

        /// <summary>
        /// 创建一个筛选执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件参数。</param>
        /// <returns>筛选执行器。</returns>
        public static IFilterExecutor Filter(this IDbEngine engine, WhereParameters where)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(where == null) throw new ArgumentNullException(nameof(where));
            return new FilterExecutor(engine, where);
        }

        #endregion

        #region Other

        private static NotSupportedException CreateKeyNotFoundException<TEntity>(string keyName)
            => new NotSupportedException($"类型“{typeof(TEntity).FullName}”执行命令时，找不到主键“{keyName}”的属性或属性值为空。");

        internal static object[] GetModifyKeyValues<TEntity>(object entity)
        {
            if(entity == null) throw new ArgumentNullException(nameof(entity));

            var mapper1 = TypeMapper.Instance<TEntity>.Mapper.ThrowWithNotFoundKeys();
            var mapper2 = TypeMapper.Create(entity.GetType());
            var keyValues = new List<object>(mapper1.KeyProperties.Length * 2);

            foreach(var keyProp1 in mapper1.KeyProperties)
            {
                keyValues.Add(keyProp1.Name);
                var keyValue = mapper2[keyProp1.Name]?.GetValue(entity);
                if(keyValue == null) throw CreateKeyNotFoundException<TEntity>(keyProp1.Name);

                keyValues.Add(keyValue);
            }
            return keyValues.ToArray();
        }

        internal static WhereParameters GetRemoveWhere<TEntity>(this IDbEngine engine, object entityOrPKValues)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(entityOrPKValues == null) throw new ArgumentNullException(nameof(entityOrPKValues));

            /*
           1、Remove(val)
           2、Remove({key=val})
           3、Remove({key=val1},val2)
           4、Remove([{key1=val2,key2=val2},{key1=val3,key2=val4}])
           */

            var mapper = TypeMapper.Instance<TEntity>.Mapper.ThrowWithNotFoundKeys();

            if(!(entityOrPKValues is string) && entityOrPKValues is Collections.IEnumerable)
            {
                entityOrPKValues = ((Collections.IEnumerable)entityOrPKValues).Cast<object>().ToArray();
            }

            var type = entityOrPKValues.GetType();

            if(mapper.KeyProperties.Length == 1)
            {//- 单个主键，1,2,3
                var keyName = mapper.KeyProperties[0].Name;

                if(type.IsArray)
                {//-3
                    var array = (Array)entityOrPKValues;
                    List<object> newValues = new List<object>(array.Length);
                    foreach(var item in array)
                    {
                        var itemType = item.GetType();
                        if(itemType.IsSimpleType()) newValues.Add(item);
                        else
                        {
                            var keyValue = TypeMapper.Create(itemType)[keyName]?.GetValue(item);
                            if(keyValue == null) throw CreateKeyNotFoundException<TEntity>(keyName);
                            newValues.Add(keyValue);
                        }
                    }
                    var builder = new SqlBuilder(engine);
                    builder.Where(keyName, keyName, newValues.ToArray());
                    return new WhereParameters(builder.WhereText, builder.Parameters);
                }
                if(!type.IsSimpleType())
                {//-2
                    entityOrPKValues = TypeMapper.Create(type)[keyName]?.GetValue(entityOrPKValues);
                    if(entityOrPKValues == null) throw CreateKeyNotFoundException<TEntity>(keyName);
                }
                //- 1,2
                var ps = new ExecuteParameterCollection(keyName, entityOrPKValues);
                return new WhereParameters(CreateWhere(engine, ps), ps);
            }
            else
            {//- 多个主键，4

                if(!type.IsArray) throw new NotSupportedException($"类型“{typeof(TEntity).FullName}”执行删除命令时，{nameof(entityOrPKValues)} 参数必须是一个数组。");

                var array = (Array)entityOrPKValues;
                var builder = new SqlBuilder(engine);
                var factory = engine.Provider.SqlFactory;
                builder.Where();
                foreach(var item in array)
                {
                    var itemType = item.GetType();
                    if(itemType.IsSimpleType()) throw new NotSupportedException($"类型“{typeof(TEntity).FullName}”包含多个主键，{nameof(entityOrPKValues)} 参数所有值都必须是一个复杂对象。");
                    else
                    {
                        var itemMapper = TypeMapper.Create(itemType);
                        builder.Or().BeginGroup();

                        foreach(var keyProp in mapper.KeyProperties)
                        {
                            var tmpKV = itemMapper[keyProp.Name]?.GetValue(item);
                            if(tmpKV == null) throw CreateKeyNotFoundException<TEntity>(keyProp.Name);
                            builder.And(keyProp.Name, tmpKV);
                        }
                        builder.EndGroup();
                    }
                }
                return new WhereParameters(builder.WhereText, builder.Parameters);
            }
        }

        internal static object[] GetKeyValues<TEntity>(string keyName, object keyValue)
        {
            if(keyValue == null) throw new ArgumentNullException(nameof(keyValue));

            var keyValues = new List<object>(2);

            if(string.IsNullOrWhiteSpace(keyName))
            {
                var mapper = TypeMapper.Instance<TEntity>.Mapper.ThrowWithNotFoundKeys();
                if(mapper.KeyProperties.Length == 1)
                {
                    keyName = mapper.KeyProperties[0].Name;
                }
                else
                {
                    var keyValueType = keyValue.GetType();
                    if(keyValueType.IsSimpleType()) throw new NotSupportedException($"类型“{typeof(TEntity).FullName}”包含多个主键，{nameof(keyValue)} 参数所有值都必须是一个复杂对象。");

                    var mapper2 = TypeMapper.Create(keyValueType);
                    foreach(var keyProp in mapper.KeyProperties)
                    {
                        keyValues.Add(keyProp.Name);
                        var tmpKV = mapper2[keyProp.Name]?.GetValue(keyValue);
                        if(tmpKV == null) throw CreateKeyNotFoundException<TEntity>(keyProp.Name);
                        keyValues.Add(tmpKV);
                    }
                }
            }

            if(keyValues.Count == 0)
            {
                keyValues.Add(keyName);
                keyValues.Add(keyValue);
            }
            return keyValues.ToArray();
        }
        
        /// <summary>
        /// 创建一个条件查询语句。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>一个条件查询语句。</returns>
        public static string CreateWhere(this IDbEngine engine, ExecuteParameterCollection ps, string binary = "AND")
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(ps == null || ps.Count == 0) return null;
            if(string.IsNullOrWhiteSpace(binary)) throw new ArgumentNullException(nameof(binary));

            var factory = engine.Provider.SqlFactory;
            var builder = new StringBuilder();
            int index = 0;
            foreach(var p in ps)
            {
                if(index++ > 0) builder.Append(' ').Append(binary).Append(' ');
                var name = factory.UnescapeName(p.Name);
                builder
                    .Append(factory.EscapeName(name, NamePoint.Field))
                    .Append('=').Append(factory.EscapeName(name, NamePoint.Value));
            }
            return builder.ToString();
        }

        #endregion
    }
}

