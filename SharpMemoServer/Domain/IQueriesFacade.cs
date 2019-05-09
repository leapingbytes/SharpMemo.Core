using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpMemoServer.Domain
{
    public interface IQueriesFacade
    {
        Task<List<Guid>> ListTables();
        
        Task<GameState> TableState(Guid tableId);

        Task<GameState> NewTableState(Guid tableId, DateTime knownStateTimeStamp);
    }
}