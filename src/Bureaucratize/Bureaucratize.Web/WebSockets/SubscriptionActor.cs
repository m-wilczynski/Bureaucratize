using System;
using System.Threading.Tasks;
using Akka.Actor;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes;
using Bureaucratize.ImageProcessing.Contracts.RemotingMessages;
using Bureaucratize.Web.Config;
using Newtonsoft.Json;

namespace Bureaucratize.Web.WebSockets
{
    public class SubscriptionActor : ReceiveActor
    {
        private readonly Guid _documentId;
        private readonly DocumentHub _hub;

        public SubscriptionActor(Guid documentId, DocumentHub hub)
        {
            if (hub == null) throw new ArgumentNullException(nameof(hub));
            _documentId = documentId;
            _hub = hub;
            Ready();
        }

        private void Ready()
        {
            DocumentSystemActors.RemoteImageProcessing.Tell(new SubscribeForDocumentOfId(_documentId));

            Receive<DocumentProcessingCompleted>(document =>
            {
                Task.Delay(500)
                    .ContinueWith(_ => _hub.SendMessageToAllAsync(JsonConvert.SerializeObject(document)));
            });

            Receive<DocumentPageProcessingCompleted>(page =>
            {
                Task.Delay(500)
                    .ContinueWith(_ => _hub.MapAndSendDocumentPage(page));
            });

            Receive<DocumentPageTextAreaProcessingCompleted>(area =>
            {
                Task.Delay(500)
                    .ContinueWith(_ => _hub.SendMessageToAllAsync(JsonConvert.SerializeObject(area)));
            });

            Receive<DocumentPageChoiceAreaProcessingCompleted>(area =>
            {
                Task.Delay(500)
                    .ContinueWith(_ => _hub.SendMessageToAllAsync(JsonConvert.SerializeObject(area)));
            });
        }
    }
}
