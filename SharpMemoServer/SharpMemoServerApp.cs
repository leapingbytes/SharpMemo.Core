using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpMemoServer.Domain;
using SharpMemoServer.Persistence;

namespace SharpMemoServer
{
    public class SharpMemoServerApp : ICommandFacade, IQueriesFacade
    {
        private readonly IGameStateReader _reader;
        private readonly IGameStateWriter _writer;
        public SharpMemoServerApp(IGameStateReader reader, IGameStateWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public async Task<GameState> Handle(Guid tableId, JoinCommand command)
        {
            Task<GameState> loadStateTask = _reader.LoadTableState(tableId);

            await loadStateTask;
            
            GameState currentState = loadStateTask.Result;
            GameState newState = CommandsHandler.Handle(currentState, command);
            
            await _writer.WriteGameState(newState);

            return newState;
        }

        public async Task<GameState> Handle(Guid tableId, GuessCommand command)
        {
            Task<GameState> loadStateTask = _reader.LoadTableState(tableId);
            
            await loadStateTask;
            
            GameState currentState = loadStateTask.Result;
            GameState newState = CommandsHandler.Handle(currentState, command);
            
            await _writer.WriteGameState(newState);

            return newState;
        }

        public async Task<GameState> Handle(Guid tableId, TimeoutCommand command)
        {
            Task<GameState> loadStateTask = _reader.LoadTableState(tableId);
            
            await loadStateTask;
            
            GameState currentState = loadStateTask.Result;
            GameState newState = CommandsHandler.Handle(currentState, command);
            
            await _writer.WriteGameState(newState);

            return newState;
        }

        public Task<List<Guid>> ListTables()
        {
            return _reader.ListTables();
        }

        public Task<GameState> TableState(Guid tableId)
        {
            return _reader.LoadTableState(tableId);
        }

        public async Task<GameState> NewTableState(Guid tableId, DateTime knownStateTimeStamp)
        {
            await _writer.WaitForNewGameState(tableId, knownStateTimeStamp);

            var loadTask = _reader.LoadTableState(tableId);

            await loadTask;

            return loadTask.Result;
        }
    }
}