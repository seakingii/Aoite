using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源查询与交互的执行器。
    /// </summary>
    public partial class DbExecutor : IDbExecutor
    {
        DbConnection _connection;
        DbTransaction _transaction;
        bool _closeAfterFinally;

        /// <summary>
        /// 获取数据源查询与交互引擎的实例。
        /// </summary>
        public IDbEngine Engine { get; }
        /// <summary>
        /// 获取执行的命令。
        /// </summary>
        public ExecuteCommand Command { get; }

        /// <summary>
        /// 初始化一个 <see cref="DbExecutor"/> 类的新实例。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="command">执行的命令。</param>
        /// <param name="connection">数据源的连接。可以为 null，表示一个新的连接。</param>
        /// <param name="transaction">数据源的事务上下文。可以为 null，表示当前交互行为不存在事务。</param>
        /// <param name="closeAfterFinally">指示当执行命令以后是否关闭连接。</param>
        public DbExecutor(IDbEngine engine, ExecuteCommand command, DbConnection connection, DbTransaction transaction, bool closeAfterFinally)
        {
            if(engine == null) throw new ArgumentNullException(nameof(engine));
            if(command == null) throw new ArgumentNullException(nameof(command));
            this.Engine = engine;
            this.Command = command;
            this._connection = connection ?? engine.Provider.CreateConnection();
            this._transaction = transaction;
            this._closeAfterFinally = closeAfterFinally;
        }

        /// <summary>
        /// 打开数据源的连接。
        /// </summary>
        protected virtual void Open() => this._connection.TryOpen();

        /// <summary>
        /// 关闭数据源的连接。
        /// </summary>
        protected virtual void Close() => this._connection.TryClose();

        /// <summary>
        /// 在引擎执行命令时发生。
        /// </summary>
        public event ExecutingEventHandler Executing;
        /// <summary>
        /// 在引擎执行命令后发生。
        /// </summary>
        public event ExecutedEventHandler Executed;

        /// <summary>
        /// 订阅命令执行前的事件。
        /// </summary>
        /// <param name="callback">事件的回调函数。</param>
        /// <returns>当前 <see cref="IDbExecutor"/></returns>
        public IDbExecutor SubExecuting(ExecutingEventHandler callback)
        {
            if(callback == null) throw new ArgumentNullException(nameof(callback));

            Executing += callback;
            return this;
        }
        /// <summary>
        /// 订阅命令执行后的事件。
        /// </summary>
        /// <param name="callback">事件的回调函数。</param>
        /// <returns>当前 <see cref="IDbExecutor"/></returns>
        public IDbExecutor SubExecuted(ExecutedEventHandler callback)
        {
            if(callback == null) throw new ArgumentNullException(nameof(callback));

            Executed += callback;
            return this;
        }

        /// <summary>
        /// 命令执行时发生。
        /// </summary>
        /// <param name="type">执行的命令类型。</param>
        /// <param name="dbCommand">执行的 <see cref="DbCommand"/>。</param>
        protected virtual void OnExecuting(ExecuteType type, DbCommand dbCommand)
        {
            this.Executing?.Invoke(this.Engine, this.Command.GetEventArgs(type, dbCommand, null));
            this.Engine.Owner.OnExecuting(this.Engine, type, this.Command, dbCommand);
        }

        /// <summary>
        /// 命令执行后发生。
        /// </summary>
        /// <param name="type">执行的命令类型。</param>
        /// <param name="result">执行的返回结果。</param>
        /// <param name="dbCommand">执行的 <see cref="DbCommand"/>。</param>
        protected virtual void OnExecuted(ExecuteType type, DbCommand dbCommand, object result)
        {
            this.Executed?.Invoke(this.Engine, this.Command.GetEventArgs(type, dbCommand, result));
            this.Engine.Owner.OnExecuted(this.Engine, type, this.Command, dbCommand, result);
        }

        /// <summary>
        /// 创建一个关联当前执行器的 <see cref="DbCommand"/> 的实例。
        /// </summary>
        /// <returns>关联当前执行器的 <see cref="DbCommand"/> 的实例。</returns>
        protected virtual DbCommand CreateDbCommand() => this.CreateDbCommand(this.Command);

        /// <summary>
        /// 指定执行命令，创建一个关联当前执行器的 <see cref="DbCommand"/> 的实例。
        /// </summary>
        /// <param name="command">执行的命令。</param>
        /// <returns>关联当前执行器的 <see cref="DbCommand"/> 的实例。</returns>
        protected virtual DbCommand CreateDbCommand(ExecuteCommand command)
        {
            var dbCommand = this.Engine.Provider.DbFactory.CreateCommand();

            dbCommand.Connection = this._connection;
            dbCommand.Transaction = this._transaction;

            var commandText = command.Text;
            if(commandText[0] == '>')//- 存储过程
            {
                commandText = commandText.Remove(0, 1);
                dbCommand.CommandType = CommandType.StoredProcedure;
            }

            dbCommand.CommandText = commandText;
            if(command.Count > 0) this.FillParameters(dbCommand, command.Parameters);
            return dbCommand;
        }

        /// <summary>
        /// 将参数集合填充到 <see cref="DbCommand"/>。
        /// </summary>
        /// <param name="command">命令对象。</param>
        /// <param name="parameters">参数集合。</param>
        protected virtual void FillParameters(DbCommand command, ExecuteParameterCollection parameters)
        {
            foreach(var p in parameters)
            {
                var dbp = p.CreateParameter(command);
                dbp.ParameterName = this.Engine.Provider.SqlFactory.EscapeName(dbp.ParameterName, NamePoint.Parameter);
                command.Parameters.Add(dbp);
            }
        }

        /// <summary>
        /// 执行数据源的查询与交互。
        /// </summary>
        /// <typeparam name="TValue">返回结果的数据类型。</typeparam>
        /// <param name="type">执行的命令类型。</param>
        /// <param name="callback">执行时的回调方法。</param>
        /// <returns>执行结果。</returns>
        protected virtual TValue Execute<TValue>(ExecuteType type, Func<DbCommand, TValue> callback)
        {
            var dbCommand = this.CreateDbCommand();
            TValue value = default(TValue);

            this.OnExecuting(type, dbCommand);

            this.Open();
            try
            {
                value = callback(dbCommand);
            }
            catch(Exception ex)
            {
                GA.OnGlobalError(this.Engine, ex);
                throw;
            }
            finally
            {
                if(this._closeAfterFinally) this.Close();
            }

            this.OnExecuted(type, dbCommand, value);

            return value;
        }

        /// <summary>
        /// 提供一个 <see cref="DbCommand"/> 的实例，创建一个关联的数据适配器。
        /// </summary>
        /// <param name="command">一个 <see cref="DbCommand"/> 的实例。</param>
        /// <returns>关联 <paramref name="command"/> 的数据适配器。</returns>
        protected virtual DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            var adapter = this.Engine.Provider.DbFactory.CreateDataAdapter();
            adapter.SelectCommand = command;
            return adapter;
        }

        private long GetTotalCount()
        {
            var newCommand = this.Command.Clone() as ExecuteCommand;
            newCommand.Text = this.Engine.Provider.SqlFactory.CreatePageTotalCountCommand(newCommand.Text);
            var dbCommand = this.CreateDbCommand(newCommand);
            var value = dbCommand.ExecuteScalar<long>();
            return value;
        }


        #region Basic

        /// <summary>
        /// 执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <param name="callback">给定的读取器委托。</param>
        public virtual void ToReader(ExecuteReaderHandler callback) => this.Execute<object>(ExecuteType.Reader, dbCommand =>
        {
            using(var reader = dbCommand.ExecuteReader())
            {
                callback(reader);
            }
            return null;
        });

        /// <summary>
        /// 执行查询命令，并执行给定的读取器的回调函数。
        /// </summary>
        /// <typeparam name="TValue">返回值的数据类型。</typeparam>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>回调结果的值。</returns>
        public virtual TValue ToReader<TValue>(ExecuteReaderHandler<TValue> callback) => this.Execute(ExecuteType.Reader, dbCommand =>
        {
            using(var reader = dbCommand.ExecuteReader())
            {
                return callback(reader);
            }
        });

        /// <summary>
        /// 执行查询命令，并返回受影响的行数。
        /// </summary>
        /// <returns>受影响的行数。</returns>
        public virtual int ToNonQuery() => this.Execute(ExecuteType.NoQuery, c => c.ExecuteNonQuery());

        /// <summary>
        /// 执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <returns>结果集中第一行的第一列。</returns>
        public object ToScalar() => this.ToScalar<object>();

        /// <summary>
        /// 指定值的数据类型，执行查询命令，并返回查询结果集中第一行的第一列。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <returns>具备强类型的结果集中第一行的第一列。</returns>
        public virtual TValue ToScalar<TValue>() => this.Execute(ExecuteType.Scalar, DbExtensions.ExecuteScalar<TValue>);

        /// <summary>
        /// 执行查询命令，并返回数据集。
        /// </summary>
        /// <returns>数据集。</returns>
        public DataSet ToDataSet() => this.ToDataSet<DataSet>();

        /// <summary>
        /// 执行查询命令，并返回自定义的数据集。
        /// </summary>
        /// <typeparam name="TDataSet">自定义的数据集类型。</typeparam>
        /// <returns>具备强类型的一个数据集。</returns>
        public virtual TDataSet ToDataSet<TDataSet>()
            where TDataSet : DataSet, new() => this.Execute(ExecuteType.DataSet, dbCommand => dbCommand.ExecuteDataSet<TDataSet>(this.CreateDataAdapter(dbCommand)));

        /// <summary>
        /// 执行查询命令，并返回表。
        /// </summary>
        /// <returns>一张表。</returns>
        public virtual PageTable ToTable() => this.Execute(ExecuteType.Table, dbCommand => dbCommand.ExecuteTable(this.CreateDataAdapter(dbCommand)));

        /// <summary>
        /// 执行分页查询命令，并返回表。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>一张包含总记录数的表。</returns>
        public virtual PageTable ToTable(int pageNumber, int pageSize = 10)
        {
            if(pageNumber < 1) pageNumber = 1;
            if(pageSize < 1) pageSize = 1;

            return this.Execute(ExecuteType.Table, dbCommand => InnerToTable(dbCommand, pageNumber, pageSize));
        }

        private PageTable InnerToTable(DbCommand dbCommand, int pageNumber, int pageSize)
        {
            this.Engine.Provider.SqlFactory.PageProcessCommand(pageNumber, pageSize, dbCommand);
            var value = dbCommand.ExecuteTable(this.CreateDataAdapter(dbCommand));
            value.Total = this.GetTotalCount();
            return value;
        }

        #endregion

        #region Custom Entity

        /// <summary>
        /// 执行查询命令，并返回实体。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>实体。</returns>
        public virtual TEntity ToEntity<TEntity>() => this.Execute(ExecuteType.Reader, DbExtensions.ExecuteEntity<TEntity>);

        /// <summary>
        /// 执行查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>实体的集合。</returns>
        public virtual List<TEntity> ToEntities<TEntity>() => this.Execute(ExecuteType.Reader, DbExtensions.ExecuteEntities<TEntity>);

        /// <summary>
        /// 执行分页查询命令，并返回实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>包含总记录数的实体的集合。</returns>
        public virtual PageData<TEntity> ToEntities<TEntity>(int pageNumber, int pageSize = 10)
        {
            if(pageNumber < 1) pageNumber = 1;
            if(pageSize < 1) pageSize = 1;

            return this.Execute(ExecuteType.Reader, dbCommand => this.InnerToEntities<TEntity>(dbCommand, pageNumber, pageSize));
        }

        private PageData<TEntity> InnerToEntities<TEntity>(DbCommand dbCommand, int pageNumber, int pageSize)
        {
            this.Engine.Provider.SqlFactory.PageProcessCommand(pageNumber, pageSize, dbCommand);
            var entities = dbCommand.ExecuteEntities<TEntity>();
            var value = new PageData<TEntity>()
            {
                Rows = entities.ToArray(),
                Total = this.GetTotalCount()
            };
            return value;
        }

        #endregion

        #region Dynamic Entity

        /// <summary>
        /// 执行查询命令，并返回匿名实体。
        /// </summary>
        /// <returns>匿名实体。</returns>
        public virtual dynamic ToEntity() => this.Execute(ExecuteType.Reader, DbExtensions.ExecuteEntity);

        /// <summary>
        /// 执行查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <returns>匿名实体的集合。</returns>
        public virtual List<dynamic> ToEntities() => this.Execute(ExecuteType.Reader, DbExtensions.ExecuteEntities);

        /// <summary>
        /// 执行分页查询命令，并返回匿名实体的集合。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>包含总记录数的匿名实体的集合。</returns>
        public virtual PageData<dynamic> ToEntities(int pageNumber, int pageSize = 10)
        {
            if(pageNumber < 1) pageNumber = 1;
            if(pageSize < 1) pageSize = 1;

            return this.Execute(ExecuteType.Reader, dbCommand => this.InnerToEntities(dbCommand, pageNumber, pageSize));
        }

        private PageData<dynamic> InnerToEntities(DbCommand dbCommand, int pageNumber, int pageSize)
        {
            this.Engine.Provider.SqlFactory.PageProcessCommand(pageNumber, pageSize, dbCommand);
            var entities = dbCommand.ExecuteEntities();
            var value = new PageData<dynamic>()
            {
                Rows = entities.ToArray(),
                Total = this.GetTotalCount()
            };
            return value;
        }

        #endregion
    }
}
