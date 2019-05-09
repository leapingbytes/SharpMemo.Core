namespace SharpMemoServer.Domain 
{
    public struct JoinCommand {
        public Player Player { get; }

        public JoinCommand( Player player) {
            Player = player;
        }
    }

    public struct GuessCommand {
        public Player Player { get; }
        public int Guess { get; }

        public GuessCommand(Player player, int guess) {
            Player = player;
            Guess = guess;
        }
    }

    public struct TimeoutCommand {
    }
}