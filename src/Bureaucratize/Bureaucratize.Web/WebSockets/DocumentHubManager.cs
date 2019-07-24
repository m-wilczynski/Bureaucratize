using System;
using System.Collections.Concurrent;
using Akka.Actor;
using Bureaucratize.Web.Config;

namespace Bureaucratize.Web.WebSockets
{
    public class DocumentHubManager
    {
        private static readonly ConcurrentDictionary<string, DocumentHub> Hubs =
            new ConcurrentDictionary<string, DocumentHub>();

        public DocumentHub GetHubForDocumentOfId(string documentGuid)
        {
            Hubs.TryGetValue(documentGuid, out var hub);
            return hub;
        }

        public DocumentHub GetOrStartHubForDocumentOfId(string documentGuid)
        {
            if (!Hubs.ContainsKey(documentGuid))
            {
                var newHub = new DocumentHub();
                DocumentSystemActors.ActorSystem.ActorOf(Props.Create(() => new SubscriptionActor(Guid.Parse(documentGuid), newHub)));
                Hubs.TryAdd(documentGuid, newHub);
            }

            Hubs.TryGetValue(documentGuid, out var hub);
            return hub;
        }
    }
}
