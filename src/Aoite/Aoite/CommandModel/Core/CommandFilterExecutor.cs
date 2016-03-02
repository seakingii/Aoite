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

        public IFilterExecutor OrderBy(params string[] fields)
        {
            this._where.OrderBy = fields.Join(", ");
            return this;
        }

        public IFilterExecutor OrderByDescending(params string[] fields)
        {
            this.OrderBy(fields.Each(f => f + " DESC"));
            return this;
        }

        public override string ToString()
        {
            return this._where.ToString();
        }

        public bool Exists<TEntity>(ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.Exists<TEntity> { Where = this._where, Tunnel = tunnel });

        public List<TEntity> FindAll<TEntity>(ICommandTunnel tunnel = null)
            => this.FindAll<TEntity, TEntity>(tunnel);

        public List<TView> FindAll<TEntity, TView>(ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.FindAll<TEntity, TView> { Where = this._where, Tunnel = tunnel });


        public PageData<TEntity> FindAll<TEntity>(IPagination page, ICommandTunnel tunnel = null)
            => this.FindAll<TEntity, TEntity>(page, tunnel);

        public PageData<TView> FindAll<TEntity, TView>(IPagination page, ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.FindAllPage<TEntity, TView> { Where = this._where, Page = page, Tunnel = tunnel });

        public TEntity FindOne<TEntity>(ICommandTunnel tunnel = null)
            => this.FindOne<TEntity, TEntity>(tunnel);

        public TView FindOne<TEntity, TView>(ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.FindOne<TEntity, TView> { Where = this._where, Tunnel = tunnel });

        public int Modify<TEntity>(object entity, ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.Modify<TEntity> { Where = this._where, Entity = entity, Tunnel = tunnel });

        public int Remove<TEntity>(ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.Remove<TEntity> { Where = this._where, Tunnel = tunnel });

        public long RowCount<TEntity>(ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.RowCount<TEntity> { Where = this._where, Tunnel = tunnel });

        public TView FindOne<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.FindOne<TEntity, TView> { Where = this._where, Select = select, Tunnel = tunnel });

        public List<TView> FindAll<TEntity, TView>(Func<TEntity, TView> select, ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.FindAll<TEntity, TView> { Where = this._where, Select = select, Tunnel = tunnel });

        public PageData<TView> FindAll<TEntity, TView>(Func<TEntity, TView> select, IPagination page, ICommandTunnel tunnel = null)
            => this._bus.Call(new CMD.FindAllPage<TEntity, TView> { Where = this._where, Select = select, Page = page, Tunnel = tunnel });
    }
}
