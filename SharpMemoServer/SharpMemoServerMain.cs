using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SharpMemoServer.Domain;
using SharpMemoServer.Persistence;
using SharpMemoServer.Persistence.InMemory;
using SharpMemoServer.Rest;

namespace SharpMemoServer
{
    class SharpMemoServerMain
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SharpMemoServerMain));

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            Log.Debug("Starting up");

            var persistence = new GameStateInMemoryRepository();
            
            IGameStateReader reader = persistence;
            IGameStateWriter writer = persistence;

            // Create 5 tables
            for (var t = 0; t < 5; t++)
            {
                writer.WriteGameState(GameState.Empty());
            }

            var app = new SharpMemoServerApp(reader, writer);
            ICommandFacade commandFacade = app;
            IQueriesFacade queriesFacade = app;

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureServices( services => services.AddSingleton<ICommandFacade>(app))
                .ConfigureServices( services => services.AddSingleton<IQueriesFacade>(app))
                .Build()
                .Run();

        }
    }
}
