using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Nano.DiagnosticLib;

namespace Nano.ActorSystemFactoryLib
{
    public class ActorSystemFactory
    {
  

        /// <summary>
        /// Any actor system passed in can be terminated if 'TerminateActorSystem()' is called on a disposable
        /// </summary>
        /// <param name="serverActorSystemName"></param>
        /// <param name="actorSystem"></param>
        /// <param name="actorSystemConfig"></param>
        public void CreateOrSetUpActorSystem(string serverActorSystemName = null, ActorSystem actorSystem = null, string actorSystemConfig = null)
        {
            var actorSystemName = "";
            actorSystemName = string.IsNullOrEmpty(serverActorSystemName) ? ConfigurationManager.AppSettings["ServerActorSystemName"] : serverActorSystemName;
            ServiceDiagnostics.Debug(() =>
            {
                InventoryServiceActorSystem = string.IsNullOrEmpty(actorSystemName)
                  ? actorSystem
                  : (string.IsNullOrEmpty(actorSystemConfig)
                      ? Akka.Actor.ActorSystem.Create(actorSystemName)
                      : Akka.Actor.ActorSystem.Create(serverActorSystemName, actorSystemConfig));
            });

            if (InventoryServiceActorSystem != null) return;
            const string message = "Invalid ActorSystemName.Please set up 'ServerActorSystemName' in the config file";
         
            throw new Exception(message);
        }

        public ActorSystem InventoryServiceActorSystem { get; set; }

        public void TerminateActorSystem()
        {
            InventoryServiceActorSystem?.Terminate().Wait();
            InventoryServiceActorSystem?.Dispose();
        }
    }
}
