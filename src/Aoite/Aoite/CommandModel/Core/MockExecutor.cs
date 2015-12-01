namespace Aoite.CommandModel
{
    class MockExecutor<TCommand> : IExecutor<TCommand> where TCommand : ICommand
    {
        private CommandExecuteHandler<TCommand> _handler;
        public MockExecutor(CommandExecuteHandler<TCommand> handler)
        {
            this._handler = handler;
        }
        public void Execute(IContext context, TCommand command)
        {
            this._handler(context, command);
        }
    }
}
