using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using MysticMind.PostgresEmbed;
using Npgsql;
using NpgsqlTypes;
using SharpMemoServer.Domain;

namespace SharpMemoServer.Persistence.PostgreSQL
{
    
    public class GameStatePostgreSQLRepository : IGameStateRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameStatePostgreSQLRepository));

        private static readonly string LoadTableStateSQL =
            "SELECT Timestamp, TableId, Players, Memo, GuessPosition FROM event_store WHERE TableId = @TableId";

        private static readonly string ListTablesSQL = "SELECT TableId FROM event_store GROUP BY 1";

        private readonly PgServer _server;

        private readonly Object _semaphore = new Object();
        
        public GameStatePostgreSQLRepository()
        {
            // THIS DOES NOT WORK. MysticMind.PostgresEmbed is for Windows ONLY :(
            _server = new MysticMind.PostgresEmbed.PgServer("9.5.5.1");
        }

        public void Start()
        {
            _server.Start();

            CreateSchema();
        }

        public async Task<GameState> LoadTableState(Guid tableId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Guid>> ListTables()
        {
            using (var conn = Connection())
            {
                var selectTask = new NpgsqlCommand("SELECT DISTINCT TableId FROM event_store", conn)
                    .ExecuteReaderAsync();
                    
                await selectTask;
                    
                List<Guid> result = new List<Guid>();

                var reader = selectTask.Result;
                while (reader.HasRows)
                {
                    reader.Read();
                    result.Add(Guid.Parse((string) reader.GetString(0)));                            
                }
                
                return result;
            }
        }
        
        public async Task<bool> WriteGameState(GameState state)
        {
            using (var conn = Connection())
            {
                var insert = new NpgsqlCommand(
                    @"INSERT INTO event_store 
                                    (Timestamp, TableId, Players, Memo, GuessPosition) 
                                VALUES 
                                    (@timestamp, @tableId, @players, @memo, @guessPosition)", 
                    conn);
                insert.Parameters.AddWithValue("timestamp", state.Timestamp);
                insert.Parameters.AddWithValue("tableId", state.TableId);
                insert.Parameters.AddWithValue("guessPosition", state.GuessPosition);
                
                insert.Parameters.Add(new NpgsqlParameter("players", NpgsqlDbType.Jsonb) { Value = state.Players });
                insert.Parameters.Add(new NpgsqlParameter("memo", NpgsqlDbType.Jsonb) { Value = state.Memo });

                var insertTask = insert.ExecuteNonQueryAsync();
                
                await insertTask;

                Monitor.PulseAll(_semaphore);

                return insertTask.IsCompleted;
            }
        }

        public async Task<bool> WaitForNewGameState(Guid tableId, DateTime knownTimeStamp)
        {
            throw new NotImplementedException();
        }

        private void CreateSchema()
        {
            using (var conn = Connection())
            {
                new Npgsql.NpgsqlCommand(
                    @"CREATE TABLE event_store(
                      Id BIGSERIAL PRIMARY KEY,
                      Timestamp timestamp,
                      TableId char(36),
                      Players JSON,
                      Memo JSON,
                      GuessPosition smallint,
                      
                      INDEX byTableId(TableId),
                      INDEX byTableIdAndTimestamp(TableId, Timestamp)
                    );",
                    conn
                ).ExecuteNonQuery();
            }
        }

        private NpgsqlConnection Connection()
        {
            string connStr = $"Server=localhost;Port={_server.PgPort};User Id=postgres;Password=test;Database=postgres";

            return new Npgsql.NpgsqlConnection(connStr);
        }
    }
}