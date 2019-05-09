using System;
using SharpMemoServer.Domain;
using Xunit;

namespace SharpMemoServer.Test.Domain
{
    public class GameStateTest
    {
        [Fact]
        public void When_emptyState_Then_playersAndMemoShouldBeEmpty() {
            var emptyState = GameState.Empty();

            Assert.NotNull(emptyState.TableId);
            Assert.NotNull(emptyState.Timestamp);

            Assert.Empty(emptyState.Players);
            Assert.Empty(emptyState.Memo);

            Assert.Equal(0, emptyState.GuessPosition);
        }

        [Fact]
        public void Given_emptyState_When_join_Then_addPlayerToList() {
            var player = new Player("someblock", Guid.NewGuid().ToString());
            var emptyState = GameState.Empty();
            var joinState = emptyState.Join(player);

            Assert.Equal(1, joinState.Players.Count);
            Assert.Equal(player, joinState.Players[0]);
        }

        [Fact]
        public void Given_stateWithPlayerAndEmptyMemo_When_guess_Then_addGuessToMemo() {
            var player = new Player("someblock", Guid.NewGuid().ToString());
            var state = GameState.Empty().Join(player);

            var newState = state.Guess(1);

            Assert.NotEmpty(newState.Players);
            Assert.NotEmpty(newState.Memo);
            Assert.Equal(0, newState.GuessPosition);
        }
    }
}
