using System;
using System.Threading.Tasks;
using SharpMemoServer.Domain;

namespace SharpMemoServer.Persistence
{
    public interface IGameStateWriter
    {
        Task<bool> WriteGameState(GameState state);
        Task<bool> WaitForNewGameState(Guid tableId, DateTime knownTimeStamp);
    }
}