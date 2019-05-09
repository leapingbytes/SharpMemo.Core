using System;
using System.Threading.Tasks;

namespace SharpMemoServer.Domain
{
    public interface ICommandFacade
    {
        Task<GameState> Handle(Guid tableId, JoinCommand command);
        Task<GameState> Handle(Guid tableId, GuessCommand command);
        Task<GameState> Handle(Guid tableId, TimeoutCommand command);
    }
}