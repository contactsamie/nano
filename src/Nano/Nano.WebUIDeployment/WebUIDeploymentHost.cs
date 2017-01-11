﻿

using System;
using System.Configuration;
using System.IO;
using Akka.Actor;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Nano.ActorSystemFactoryLib;
using Nano.WebUIHost;
using NLog;
using Owin;

namespace Nano.WebUIDeployment
{
    public class WebUIDeploymentHost
    {
        public WebUIDeploymentHost()
        {
            ActorSystemFactory = new ActorSystemFactory();
        }

        private ActorSystemFactory ActorSystemFactory { set; get; }
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public void Start(string serverEndPoint = null, string serverActorSystemName = null,
            ActorSystem serverActorSystem = null, string serverActorSystemConfig = null)
        {
            try
            {
                ActorSystemFactory.CreateOrSetUpActorSystem( serverActorSystemName,  serverActorSystem, actorSystemConfig: serverActorSystemConfig);
               
                const string message = "signalRInventoryQueryActor created !!!!";
                Log.Debug(message);
              

                Log.Debug("Starting inventory service ...");
                serverEndPoint = serverEndPoint ?? ConfigurationManager.AppSettings["ServerEndPoint"];

                if (!string.IsNullOrEmpty(serverEndPoint))
                {
                    OwinRef = WebApp.Start(serverEndPoint, (appBuilder) =>
                    {
                        if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/web"))
                        {
                            var builder = new ContainerBuilder();
                          //builder.Register(c => signalRInventoryQueryActorRef).ExternallyOwned();
                            // Register your SignalR hubs.
                            builder.RegisterType<NanoServiceHub>().ExternallyOwned();

                            var container = builder.Build();
                            //var config = new HubConfiguration {Resolver = new AutofacDependencyResolver(container)};
                            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);
                            appBuilder.UseAutofacMiddleware(container);

                            appBuilder.MapSignalR();

                            var fileSystem = new PhysicalFileSystem(AppDomain.CurrentDomain.BaseDirectory + "/web");
                            var options = new FileServerOptions
                            {
                                EnableDirectoryBrowsing = true,
                                FileSystem = fileSystem,
                                EnableDefaultFiles = true
                            };

                            appBuilder.UseFileServer(options);
                        }

                        //  InventoryServiceSignalRContext.Push();
                    });
                }

                Log.Debug("WebUIDeployment initialized successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to start inventory service UI");
                throw;
            }
        }

        protected IDisposable OwinRef { get; set; }

        public void Stop()
        {
            OwinRef?.Dispose();
        }
    }
}
