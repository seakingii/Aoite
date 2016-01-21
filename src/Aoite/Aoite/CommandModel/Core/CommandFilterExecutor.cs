using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoite.Data;

namespace Aoite.CommandModel
{
    partial class CommandFilterExecutor : IFilterExecutor
    {
        ICommandBus _bus;
        WhereParameters _where;

        public CommandFilterExecutor(ICommandBus bus, WhereParameters where)
        {
            this._bus = bus;
            this._where = where;
        }

        public bool Exists<TEntity>(ICommandTunnel tunnel = null)
            => this._bus.Execute(new CMD.Exists<TEntity> { Where = this._where, Tunnel = tunnel }).Result;

        public List<TEntity> FindAll<TEntity>(ICommandTunnel tunnel = null)
            => this.FindAll<TEntity, TEntity>(tunnel);

        public List<TView> FindAll<TEntity, TView>(ICommandTunnel tunnel = null)
            => this._bus.Execute(new CMD.FindAll<TEntity, TView> { Where = this._where, Tunnel = tunnel }).Result;


        public PageData<TEntity> FindAll<TEntity>(IPagination page, ICommandTunnel tunnel = null)
            => this.FindAll<TEntity, TEntity>(page, tunnel);

        public PageData<TView> FindAll<TEntity, TView>(IPagination page, ICommandTunnel tunnel = null)
            => this._bus.Execute(new CMD.FindAllPage<TEntity, TView> { Where = this._where, Page = page, Tunnel = tunnel }).Result;

        public TEntity FindOne<TEntity>(ICommandTunnel tunnel = null)
            => this.FindOne<TEntity, TEntity>(tunnel);

        public TView FindOne<TEntity, TView>(ICommandTunnel tunnel = null)
            => this._bus.Execute(new CMD.FindOne<TEntity, TView> { Where = this._where, Tunnel = tunnel }).Result;

        public int Modify<TEntity>(object entity, ICommandTunnel tunnel = null)
            => this._bus.Execute(new CMD.Modify<TEntity> { Where = this._where, Entity = entity, Tunnel = tunnel }).Result;

        public int Remove<TEntity>(ICommandTunnel tunnel = null)
            => this._bus.Execute(new CMD.Remove<TEntity> { Where = this._where, Tunnel = tunnel }).Result;

        public long RowCount<TEntity>(ICommandTunnel tunnel = null)
            => this._bus.Execute(new CMD.RowCount<TEntity> { Where = this._where, Tunnel = tunnel }).Result;
    }
}
