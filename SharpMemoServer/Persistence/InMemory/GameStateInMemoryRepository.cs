using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using SharpMemoServer.Domain;

namespace SharpMemoServer.Persistence.InMemory
{
    public class GameStateInMemoryRepository : IGameStateRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameStateInMemoryRepository));

        private static readonly string LoadTableStateSQL =
            "SELECT Timestamp, TableId, Players, Memo, GuessPosition FROM event_store WHERE TableId = @TableId";

        private static readonly string ListTablesSQL = "SELECT TableId FROM event_store GROUP BY 1";

        
        public async Task<GameState> LoadTableState(Guid tableId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Guid>> ListTables()
        {
            throw new NotImplementedException();
        }
        
        public async Task<bool> WriteGameState(GameState state)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> WaitForNewGameState(Guid tableId, DateTime knownTimeStamp)
        {
            throw new NotImplementedException();
        }
    }
}