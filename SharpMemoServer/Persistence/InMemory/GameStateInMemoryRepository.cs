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

        private readonly List<GameState> _repository;

        public GameStateInMemoryRepository()
        {
            _repository = new List<GameState>();
        }
        
        public async Task<GameState> LoadTableState(Guid tableId)
        {
            return _repository.Find((state => state.TableId.Equals(tableId)));
        }

        public async Task<List<Guid>> ListTables()
        {
            var result = new List<Guid>();

            _repository.ForEach(state =>
            {
                if (!result.Contains(state.TableId))
                {
                    result.Add(state.TableId);
                }
            });

            return result;
        }
        
        public async Task<bool> WriteGameState(GameState state)
        {
            _repository.Insert(0, state);

            return true;
        }

        public async Task<bool> WaitForNewGameState(Guid tableId, DateTime knownTimeStamp)
        {
            throw new NotImplementedException();
        }
    }
}