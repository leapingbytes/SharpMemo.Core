using System;
using System.Collections.Generic;
using System.Collections.Immutable;
//using Butterfly.Core.Util;

namespace SharpMemoServer.Domain
{
    public struct Player
    {
        public string ScreenName { get; }

        public string SessionId { get; }

        public Player(string screenName, string sessionId)
        {
            ScreenName = screenName;
            SessionId = sessionId;
        }
    }
    public struct GameState
    {
        public DateTime Timestamp { get; }
        public Guid TableId { get; }
        public ImmutableList<Player> Players { get; }
        public ImmutableList<int> Memo { get; }
        public int GuessPosition { get; }

        private GameState(Guid tableId, ImmutableList<Player> players, ImmutableList<int> memo, int guessPosition) :
            this(DateTime.Now, tableId, players, memo, guessPosition)
        {
        }

        private GameState(DateTime timestamp, Guid tableId, ImmutableList<Player> players, ImmutableList<int> memo, int guessPosition)
        {
            Timestamp = timestamp;

            TableId = tableId;
            Players = players;
            Memo = memo;
            GuessPosition = guessPosition;
        }

        public static GameState Empty()
        {
            return new GameState(Guid.NewGuid(), ImmutableList.Create<Player>(), ImmutableList.Create<int>(), 0);
        }

        public GameState Join(Player player)
        {
            return new GameState(TableId, Players.Add(player), Memo, GuessPosition);
        }

        public GameState Guess(int guess)
        {
            var player0 = Players[0];
            var otherPlayers = Players.Remove(player0);

            if (Memo.Count <= GuessPosition)
            {
                return new GameState(TableId, otherPlayers.Add(player0), Memo.Add(guess), 0);
            }

            return Memo[GuessPosition] == guess
                ? new GameState(TableId, Players, Memo, GuessPosition + 1)
                : new GameState(TableId, otherPlayers, Memo, 0)
                ;
        }

        public GameState Timeout()
        {
            return new GameState(TableId, Players.RemoveAt(0), Memo, 0);
        }

//        public static GameState FromDictionary(Dictionary<string,object> dictionary)
//        {
//            return new GameState(
//                (DateTime) dictionary["Timestamp"],
//                Guid.Parse((string) dictionary["TableId"]), 
//                JsonUtil.Deserialize<ImmutableList<Player>>((string) dictionary["Players"]), 
//                JsonUtil.Deserialize<ImmutableList<int>>((string)dictionary["Memo"]), 
//                Convert.ToInt32(dictionary["GuessPosition"])
//            );
//        }
//
//        public Dictionary<string, object> ToDictionary()
//        {
//            return new Dictionary<string, object>
//            {
//                { "Timestamp", Timestamp},
//                { "TableId", TableId},
//                { "Players", JsonUtil.Serialize(Players) },
//                { "Memo", JsonUtil.Serialize(Memo) },
//                { "GuessPosition", GuessPosition },
//            };
//        }
    }
}