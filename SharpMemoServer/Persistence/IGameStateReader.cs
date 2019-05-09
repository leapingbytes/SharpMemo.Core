using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpMemoServer.Domain;

namespace SharpMemoServer.Persistence
{
    public interface IGameStateReader
    {
        Task<GameState> LoadTableState(Guid tableId);

        Task<List<Guid>> ListTables();
    }
}