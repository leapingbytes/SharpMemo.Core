using log4net;

namespace SharpMemoServer.Domain
{
    static class CommandsHandler 
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommandsHandler));

        public static GameState Handle(GameState state, JoinCommand command)
        {
            GameState newState = state;
            if (!state.Players.Contains(command.Player))
            {
                newState = state.Join(command.Player);
            }
            
            Log.Info("JoinCommand: state: " + state + " command: " + command + " newState: " + newState);

            return newState;
        }

        public static GameState Handle(GameState state, GuessCommand command)
        {
            GameState newState = state;
            
            if (state.Players[0].SessionId == command.Player.SessionId)
            {
                newState = state.Guess(command.Guess);
            }

            Log.Info("GuessCommand: state: " + state + " command: " + command + " newState: " + newState);

            return newState;
        }

        public static GameState Handle(GameState state, TimeoutCommand command)
        {
            GameState newState = state;
            
            if (state.Players.Count > 1)
            {
                newState = state.Timeout();
            }

            Log.Info("TimeoutCommand: state: " + state + " command: " + command + " newState: " + newState);

            return newState;
        }
    }
}