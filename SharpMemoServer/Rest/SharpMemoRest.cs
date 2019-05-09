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


//        private void InitializeMonitoringApis()
//        {
//            _ctx.WebApi.OnGet("/sharp-memo/v1/ping", (req, res) => res.WriteAsTextAsync("pong\n"));
//            _ctx.WebApi.OnGet("/{fileName}", async (req, res) =>
//            {
//                var fileName = req.PathParams["fileName"];
//                RedHttpServerWebResponse redResponse = res as RedHttpServerWebResponse;
//
//
//                await redResponse.response.SendFile("../SharpMemoUI.HTML/" + fileName);
//                    
//                Task.Run(() =>
//                    {
//                        redResponse.response.Closed = true;
//                    });                    
//            });
//        }
//        
//        private void InitializeQueryApis()
//        {
//            _ctx.WebApi.OnGet("/sharp-memo/v1/tables", async (req, res) =>
//            {
//                await _queriesFacade.ListTables().ContinueWith( a => WriteAsJsonAsync(res, a.Result));
//            });
//            
//            _ctx.WebApi.OnGet("/sharp-memo/v1/table/{tableId}/{timeStamp}", async (req, res) =>
//            {
//                Guid tableId = Guid.Parse(req.PathParams["tableId"]);
//                long timeStamp = Int64.Parse(req.PathParams["timeStamp"]);
//
//                await _queriesFacade.NewTableState(tableId, DateTime.FromFileTimeUtc(timeStamp)).ContinueWith( a => WriteAsJsonAsync(res, a.Result));
//            });
//
//            _ctx.WebApi.OnGet("/sharp-memo/v1/table/{tableId}", async (req, res) =>
//            {
//                Guid tableId = Guid.Parse(req.PathParams["tableId"]);
//
//                await _queriesFacade.TableState(tableId).ContinueWith( a => WriteAsJsonAsync(res, a.Result));
//            });
//        }
//
//        private void InitializeCommandApIs()
//        {
//            _ctx.WebApi.OnPost("/sharp-memo/v1/table/{tableId}/join", async (req, res) =>
//            {
//                Guid tableId = Guid.Parse(req.PathParams["tableId"]);
//                var joinCommand = req.ParseAsJsonAsync<JoinCommand>();
//                await joinCommand;
//                await _commandFacade.Handle(tableId, joinCommand.Result).ContinueWith( a => WriteAsJsonAsync(res, a.Result));
//            });
//            _ctx.WebApi.OnPost("/sharp-memo/v1/table/{tableId}/guess", async (req, res) =>
//            {
//                Guid tableId = Guid.Parse(req.PathParams["tableId"]);
//                var guessCommand = req.ParseAsJsonAsync<GuessCommand>();
//                await guessCommand;
//                await _commandFacade.Handle(tableId, guessCommand.Result).ContinueWith( a => WriteAsJsonAsync(res, a.Result));
//            });
//        }
//
//        private Task WriteAsJsonAsync(IHttpResponse response, object value)
//        {
//            RedHttpServerWebResponse redResponse = response as RedHttpServerWebResponse;
//            
//            redResponse.response.AddHeader("Access-Control-Allow-Origin", "*");
//            return response.WriteAsJsonAsync(value);
//        }
    }
}