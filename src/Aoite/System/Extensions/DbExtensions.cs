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

        internal static int ExecuteAED<TEntity>(this IDbEngine engine, Func<TypeMapper, ExecuteCommand> callback)
        {
            var command = callback(TypeMapper.Instance<TEntity>.Mapper);
            return engine.Execute(command).ToNonQuery();
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
        public static PageData<dynamic> ToEntities(this IDbExecutor executor, IPagination page)
        {
            if(executor == null) throw new ArgumentNullException(nameof(executor));
            if(page == null) throw new ArgumentNullException(nameof(page));
            return executor.ToEntities(page.PageNumber, page.PageSize);
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
        /// 执行一个插入的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询结果。</returns>
        public static int AddAnonymous<TEntity>(this IDbEngine engine, object entity, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(entity == null) throw new ArgumentNullException(nameof(entity));
            return engine.ExecuteAED<TEntity>(mapper => engine.Provider.CreateInsertCommand(mapper, entity, tableName));
        }

        /// <summary>
        /// 执行一个插入的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询结果。</returns>
        public static int Add<TEntity>(this IDbEngine engine, TEntity entity, string tableName = null)
        {
            return AddAnonymous<TEntity>(engine, entity, tableName);
        }

        #endregion

        #region Modify

        /// <summary>
        /// 执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询结果。</returns>
        public static int ModifyAnonymous<TEntity>(this IDbEngine engine, object entity, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(entity == null) throw new ArgumentNullException(nameof(entity));
            return engine.ExecuteAED<TEntity>(mapper => engine.Provider.CreateUpdateCommand(mapper, entity, tableName));
        }

        /// <summary>
        /// 执行一个更新的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询结果。</returns>
        public static int Modify<TEntity>(this IDbEngine engine, TEntity entity, string tableName = null)
        {
            return ModifyAnonymous<TEntity>(engine, entity, tableName);
        }

        #endregion

        #region ModifyWhere

        /// <summary>
        /// 提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static int ModifyWhere<TEntity>(this IDbEngine engine, object entity, object objectInstance)
        {
            return ModifyWhere<TEntity>(engine, entity, new ExecuteParameterCollection(objectInstance));
        }

        /// <summary>
        /// 提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>  
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="ps">参数集合实例。</param>
        public static int ModifyWhere<TEntity>(this IDbEngine engine, object entity, ExecuteParameterCollection ps)
        {
            return ModifyWhere<TEntity>(engine, entity, CreateWhere(engine, ps), ps);
        }
        /// <summary>
        /// 提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static int ModifyWhere<TEntity>(this IDbEngine engine, object entity, string where, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(entity == null) throw new ArgumentNullException(nameof(entity));
            if(ps == null) ps = new ExecuteParameterCollection();

            var mapper1 = TypeMapper.Instance<TEntity>.Mapper;
            var mapper2 = TypeMapper.Create(entity.GetType());
            var provider = engine.Provider;
            var index = 0;
            var setBuilder = new StringBuilder("UPDATE ")
                                .Append(provider.EscapeName(mapper1.Name, NamePoint.Table))
                                .Append(" SET ");

            foreach(var property2 in mapper2.Properties)
            {
                if(property2.IsIgnore || property2.IsKey) continue;
                if(!mapper1.Contains(property2.Name)) throw new NotSupportedException("表 {0} 没有属性 {1}。".Fmt(mapper1.Name, property2.Name));
                if(index > 0) setBuilder.Append(',');
                var pName = "_value_" + index;
                setBuilder.Append(provider.EscapeName(property2.Name, NamePoint.Field))
                          .Append('=')
                          .Append(provider.EscapeName(pName, NamePoint.Value));
                ps.Add(provider.EscapeName(pName, NamePoint.Parameter), property2.GetValue(entity));
                index++;
            }
            if(where != null) setBuilder.Append(" WHERE ").Append(where);
            return engine.Execute(setBuilder.ToString(), ps).ToNonQuery();
        }

        #endregion

        #region Remove

        /// <summary>
        /// 执行一个删除的命令，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entityOrPKValues">实体的实例对象，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询结果。</returns>
        public static int RemoveAnonymous<TEntity>(this IDbEngine engine, object entityOrPKValues, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(entityOrPKValues == null) throw new ArgumentNullException(nameof(entityOrPKValues));
            return engine.ExecuteAED<TEntity>(mapper => engine.Provider.CreateDeleteCommand(mapper, entityOrPKValues, tableName));
        }

        /// <summary>
        /// 执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>一个查询结果。</returns>
        public static int Remove<TEntity>(this IDbEngine engine, TEntity entity, string tableName = null) => RemoveAnonymous<TEntity>(engine, entity, tableName);

        #endregion

        #region RemoveWhere

        /// <summary>
        /// 提供匹配条件，执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static int RemoveWhere<TEntity>(this IDbEngine engine, object objectInstance) => RemoveWhere<TEntity>(engine, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 提供匹配条件，执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>  
        /// <param name="ps">参数集合实例。</param>
        public static int RemoveWhere<TEntity>(this IDbEngine engine, ExecuteParameterCollection ps) => RemoveWhere<TEntity>(engine, CreateWhere(engine, ps), ps);

        /// <summary>
        /// 提供匹配条件，执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static int RemoveWhere<TEntity>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));

            var mapper = TypeMapper.Instance<TEntity>.Mapper;
            var setBuilder = new StringBuilder("DELETE ")
                                .Append(engine.Provider.EscapeName(mapper.Name, NamePoint.Table));
            if(where != null)
            {
                setBuilder.Append(" WHERE ").Append(where);
            }
            return engine.Execute(setBuilder.ToString(), ps).ToNonQuery();

        }

        #endregion

        #region Select

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
            return new SqlBuilder(engine).Select(fields).From(engine.Provider.EscapeName(TypeMapper.Instance<TEntity>.Mapper.Name, NamePoint.Table));
        }

        #endregion

        #region FineOne

        /// <summary>
        /// 获取指定 <paramref name="id"/> 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="id">字段“Id”的值。</param>
        public static TEntity FindOne<TEntity>(this IDbEngine engine, object id) => FindOne<TEntity, TEntity>(engine, id);

        /// <summary>
        /// 获取指定 <paramref name="keyName"/> 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static TEntity FindOne<TEntity>(this IDbEngine engine, string keyName, object keyValue) => FindOne<TEntity, TEntity>(engine, keyName, keyValue);

        /// <summary>
        /// 获取指定 <paramref name="id"/> 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="id">字段“Id”的值。</param>
        public static TView FindOne<TEntity, TView>(this IDbEngine engine, object id) => FindOne<TEntity, TView>(engine, null, id);

        /// <summary>
        /// 获取指定 <paramref name="keyName"/> 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static TView FindOne<TEntity, TView>(this IDbEngine engine, string keyName, object keyValue)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(keyValue == null) throw new ArgumentNullException(nameof(keyValue));

            var provider = engine.Provider;
            var mapper = TypeMapper.Instance<TEntity>.Mapper;
            if(string.IsNullOrWhiteSpace(keyName))
            {
                var prop = mapper.Properties.FirstOrDefault(p => p.IsKey);
                if(prop == null) keyName = DefaultKeyName;
                else keyName = prop.Name;
            }
            var fields = CreateFields<TEntity, TView>(engine);

            return engine.Execute("SELECT " + fields + " FROM " + provider.EscapeName(mapper.Name, NamePoint.Table)
                + " WHERE " + provider.EscapeName(keyName, NamePoint.Field) + "=" + provider.EscapeName("pk", NamePoint.Value)
                , provider.EscapeName("pk", NamePoint.Parameter), keyValue).ToEntity<TView>();
        }

        #endregion

        #region FindOneWhere

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static TEntity FindOneWhere<TEntity>(this IDbEngine engine, object objectInstance) => FindOneWhere<TEntity, TEntity>(engine, objectInstance);

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static TEntity FindOneWhere<TEntity>(this IDbEngine engine, ExecuteParameterCollection ps = null) => FindOneWhere<TEntity, TEntity>(engine, ps);

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static TEntity FindOneWhere<TEntity>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null) => FindOneWhere<TEntity, TEntity>(engine, where, ps);

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static TView FindOneWhere<TEntity, TView>(this IDbEngine engine, object objectInstance) => FindOneWhere<TEntity, TView>(engine, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static TView FindOneWhere<TEntity, TView>(this IDbEngine engine, ExecuteParameterCollection ps = null) => FindOneWhere<TEntity, TView>(engine, CreateWhere(engine, ps), ps);

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static TView FindOneWhere<TEntity, TView>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));

            var m1 = TypeMapper.Instance<TEntity>.Mapper;
            var fields = CreateFields<TEntity, TView>(engine);
            var commandText = "SELECT TOP 1 " + fields + " FROM " + engine.Provider.EscapeName(TypeMapper.Instance<TEntity>.Mapper.Name, NamePoint.Table);
            if(!string.IsNullOrWhiteSpace(where)) commandText += " WHERE " + where;
            return engine.Execute(commandText, ps).ToEntity<TView>();
        }

        #endregion

        #region FindAllWhere

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static List<TEntity> FindAllWhere<TEntity>(this IDbEngine engine, object objectInstance) => FindAllWhere<TEntity, TEntity>(engine, objectInstance);

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static List<TEntity> FindAllWhere<TEntity>(this IDbEngine engine, ExecuteParameterCollection ps = null) => FindAllWhere<TEntity, TEntity>(engine, ps);

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static List<TEntity> FindAllWhere<TEntity>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null) => FindAllWhere<TEntity, TEntity>(engine, where, ps);

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static List<TView> FindAllWhere<TEntity, TView>(this IDbEngine engine, object objectInstance) => FindAllWhere<TEntity, TView>(engine, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static List<TView> FindAllWhere<TEntity, TView>(this IDbEngine engine, ExecuteParameterCollection ps = null) => FindAllWhere<TEntity, TView>(engine, CreateWhere(engine, ps), ps);

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static List<TView> FindAllWhere<TEntity, TView>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));

            var m1 = TypeMapper.Instance<TEntity>.Mapper;
            var fields = CreateFields<TEntity, TView>(engine);
            var commandText = "SELECT " + fields + " FROM " + engine.Provider.EscapeName(TypeMapper.Instance<TEntity>.Mapper.Name, NamePoint.Table);
            if(!string.IsNullOrWhiteSpace(where)) commandText += " WHERE " + where;
            return engine.Execute(commandText, ps).ToEntities<TView>();
        }

        #endregion

        #region FindAllPage

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static PageData<TEntity> FindAllPage<TEntity>(this IDbEngine engine, IPagination page, object objectInstance) => FindAllPage<TEntity, TEntity>(engine, page, objectInstance);

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="ps">参数集合实例。</param>
        public static PageData<TEntity> FindAllPage<TEntity>(this IDbEngine engine, IPagination page, ExecuteParameterCollection ps = null) => FindAllPage<TEntity, TEntity>(engine, page, ps);

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="ps">参数集合实例。</param>
        public static PageData<TEntity> FindAllPage<TEntity>(this IDbEngine engine, IPagination page, string where, ExecuteParameterCollection ps = null) => FindAllPage<TEntity, TEntity>(engine, page, where, ps);

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static PageData<TView> FindAllPage<TEntity, TView>(this IDbEngine engine, IPagination page, object objectInstance) => FindAllPage<TEntity, TView>(engine, page, new ExecuteParameterCollection(objectInstance));

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="ps">参数集合实例。</param>
        public static PageData<TView> FindAllPage<TEntity, TView>(this IDbEngine engine, IPagination page, ExecuteParameterCollection ps = null) => FindAllPage<TEntity, TView>(engine, page, CreateWhere(engine, ps), ps);

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static PageData<TView> FindAllPage<TEntity, TView>(this IDbEngine engine, IPagination page, string where, ExecuteParameterCollection ps = null)
        {
            if(page == null) throw new ArgumentNullException(nameof(page));
            if(engine == null) throw new ArgumentNullException(nameof(engine));

            var m1 = TypeMapper.Instance<TEntity>.Mapper;
            var fields = CreateFields<TEntity, TView>(engine);
            var commandText = "SELECT " + fields + " FROM " + engine.Provider.EscapeName(TypeMapper.Instance<TEntity>.Mapper.Name, NamePoint.Table);
            if(!string.IsNullOrWhiteSpace(where)) commandText += " WHERE " + where;
            return engine.Execute(commandText, ps).ToEntities<TView>(page);
        }

        #endregion

        #region Exists

        /// <summary>
        /// 判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static bool Exists<TEntity>(this IDbEngine engine, object keyValue) => Exists<TEntity>(engine, null, keyValue);

        /// <summary>
        /// 判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static bool Exists<TEntity>(this IDbEngine engine, string keyName, object keyValue)
        {
            if(string.IsNullOrWhiteSpace(keyName))
            {
                var prop = TypeMapper.Instance<TEntity>.Mapper.Properties.FirstOrDefault(p => p.IsKey);
                if(prop == null) keyName = DefaultKeyName;
                else keyName = prop.Name;
            }
            return ExistsWhere<TEntity>(engine, new ExecuteParameterCollection(keyName, keyValue));
        }

        #endregion

        #region ExistsWhere

        /// <summary>
        /// 判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static bool ExistsWhere<TEntity>(this IDbEngine engine, object objectInstance)
        {
            if(objectInstance == null) throw new ArgumentNullException(nameof(objectInstance));
            return ExistsWhere<TEntity>(engine, new ExecuteParameterCollection(objectInstance));
        }

        /// <summary>
        /// 判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static bool ExistsWhere<TEntity>(this IDbEngine engine, ExecuteParameterCollection ps)
        {
            if(ps == null || ps.Count == 0) throw new ArgumentNullException(nameof(ps));
            return ExistsWhere<TEntity>(engine, CreateWhere(engine, ps), ps);
        }

        /// <summary>
        /// 判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static bool ExistsWhere<TEntity>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));

            var commandText = "SELECT 1 FROM "
                + engine.Provider.EscapeName(TypeMapper.Instance<TEntity>.Mapper.Name, NamePoint.Table);
            if(!string.IsNullOrWhiteSpace(where)) commandText += " WHERE " + where;
            var r = engine.Execute(commandText, ps).ToScalar();
            return r != null;
        }

        #endregion

        #region RowCount

        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>一个查询结果。</returns>
        public static long RowCount<TEntity>(this IDbEngine engine, object objectInstance)
        {
            if(objectInstance == null) throw new ArgumentNullException(nameof(objectInstance));
            return RowCount<TEntity>(engine, new ExecuteParameterCollection(objectInstance));
        }

        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合。</param>
        /// <returns>一个查询结果。</returns>
        public static long RowCount<TEntity>(this IDbEngine engine, ExecuteParameterCollection ps)
            => RowCount<TEntity>(engine, CreateWhere(engine, ps));

        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">筛选条件，不含 WHERE 关键字。</param>
        /// <param name="ps">参数集合。</param>
        /// <returns>一个查询结果。</returns>
        public static long RowCount<TEntity>(this IDbEngine engine, string where = null, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));

            var commandText = "SELECT COUNT(*) FROM " + engine.Provider.EscapeName(TypeMapper.Instance<TEntity>.Mapper.Name, NamePoint.Table);
            if(!string.IsNullOrWhiteSpace(where)) commandText += " WHERE " + where;
            return engine.Execute(commandText, ps).ToScalar<long>();
        }

        #endregion

        #region Other

        /// <summary>
        /// 创建指定视图类型的字段列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>一个字段的列表。</returns>
        public static string CreateFields<TEntity, TView>(this IDbEngine engine)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));

            string fields = "*";
            var m1 = TypeMapper.Instance<TEntity>.Mapper;
            if(m1.Type != typeof(TView))
            {
                var provider = engine.Provider;
                var m2 = TypeMapper.Instance<TView>.Mapper;
                List<string> fieldList = new List<string>();
                foreach(var mp2 in m2.Properties)
                {
                    if(mp2.IsIgnore) continue;
                    //if(!m1.Contains(mp2.Name)) continue;
                    fieldList.Add(provider.EscapeName(mp2.Name, NamePoint.Field));
                }
                fields = fieldList.ToArray().Join();
            }
            return fields;
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

            var provider = engine.Provider;
            var builder = new StringBuilder();
            int index = 0;
            foreach(var p in ps)
            {
                if(index++ > 0) builder.Append(' ').Append(binary).Append(' ');
                builder
                    .Append(provider.EscapeName(p.Name, NamePoint.Field))
                    .Append('=').Append(provider.EscapeName(p.Name, NamePoint.Value));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取最后递增序列值。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>一个结果。</returns>
        public static long GetLastIdentity<TEntity>(this IDbEngine engine)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            var command = engine.Provider.CreateLastIdentityCommand(TypeMapper.Instance<TEntity>.Mapper);
            return engine.Execute(command).ToScalar<long>();
        }

        #endregion

        #endregion
    }
}
