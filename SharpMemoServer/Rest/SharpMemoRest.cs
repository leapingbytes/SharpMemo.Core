using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using SharpMemoServer.Domain;

namespace SharpMemoServer.Rest
{
    [Route("sharp-memo/v1")]
    [ApiController]
    public class SharpMemoController : ControllerBase
    {
        private readonly IQueriesFacade _queriesFacade;
        private readonly ICommandFacade _commandFacade;

        public SharpMemoController(ICommandFacade commandFacade, IQueriesFacade queriesFacade)
        {
            _commandFacade = commandFacade;
            _queriesFacade = queriesFacade;
        }

        [HttpGet("ping")]
        public ActionResult<string> Ping()
        {
            return "pong\n";
        }

        [HttpGet("tables")]
        public ActionResult<List<Guid>> ListTables()
        {
            return _queriesFacade.ListTables().Result;
        }

        [HttpGet("table/{tableId}")]
        public ActionResult<GameState> LoadTableState(string tableId)
        {
            return _queriesFacade.TableState(Guid.Parse(tableId)).Result;
        }

        [HttpGet("table/{tableId}/{timeStamp}")]
        public ActionResult<GameState> NewTableState(string tableId, double timeStamp)
        {
            var dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            return _queriesFacade.NewTableState(Guid.Parse(tableId), dateTime.AddMilliseconds(timeStamp)).Result;
        }

        [HttpPost("table/{tableId}/join")]
        public ActionResult<GameState> Join(string tableId, [FromBody] JoinCommand joinCommand)
        {
            return _commandFacade.Handle(Guid.Parse(tableId), joinCommand).Result;
        }

        [HttpPost("table/{tableId}/guess")]
        public ActionResult<GameState> Guess(string tableId, [FromBody] GuessCommand guessCommand)
        {
            return _commandFacade.Handle(Guid.Parse(tableId), guessCommand).Result;
        }
    }
}