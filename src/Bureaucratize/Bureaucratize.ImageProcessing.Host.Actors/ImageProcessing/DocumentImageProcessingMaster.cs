/*
   Copyright (c) 2018 Michał Wilczyński

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes;
using Bureaucratize.ImageProcessing.Contracts.RemotingMessages;

namespace Bureaucratize.ImageProcessing.Host.Actors.ImageProcessing
{
    public class DocumentImageProcessingMaster : ReceiveActor
    {
        public const string DocumentProcessorsRouterName = "document-processors";
        protected IActorRef DocumentProcessorsRouter;
        protected IDictionary<Guid, DocumentSubscription> SubscriptionsByDocumentId = new Dictionary<Guid, DocumentSubscription>();

        public DocumentImageProcessingMaster()
        {
            Ready();
        }

        protected override void PreStart()
        {
            DocumentProcessorsRouter = Context.Child(DocumentProcessorsRouterName).Equals(ActorRefs.Nobody)
                ? Context.ActorOf(Context.DI().Props<DocumentImagesProcessor>()
                                              .WithRouter(new RoundRobinPool(2)), "document-processor")
                : Context.Child(DocumentProcessorsRouterName);
        }

        private void Ready()
        {
            Receive<SubscribeForDocumentOfId>(sub =>
            {
                if (SubscriptionsByDocumentId.ContainsKey(sub.DocumentId))
                {
                    SubscriptionsByDocumentId[sub.DocumentId].Subscribers.Add(Sender);
                }
            });

            Receive<ProcessDocumentOfIdRequest>(req =>
            {
                if (SubscriptionsByDocumentId.ContainsKey(req.DocumentId))
                {
                    SubscriptionsByDocumentId[req.DocumentId].Subscribers.Add(Sender);
                }
                else
                {
                    SubscriptionsByDocumentId.Add(req.DocumentId, new DocumentSubscription(req, Sender));
                    DocumentProcessorsRouter.Tell(req);
                }

                Sender.Tell(req.Accepted());
            });

            Receive<DocumentProcessingCompleted>(page =>
            {
                SubscriptionsByDocumentId.TryGetValue(page.DocumentId, out var subscription);
                if (subscription == null) return;
                foreach (var subscriber in subscription.Subscribers)
                {
                    subscriber.Tell(page);
                    Console.WriteLine($"Document {page.DocumentId} completed");
                }
            });

            Receive<DocumentPageProcessingCompleted>(page =>
            {
                SubscriptionsByDocumentId.TryGetValue(page.DocumentId, out var subscription);
                if (subscription == null)
                    return;
                foreach (var subscriber in subscription.Subscribers)
                {
                    subscriber.Tell(page);
                    Console.WriteLine($"Document {page.DocumentId} completed page: {page.PageNumber}");
                }
            });

            Receive<DocumentPageTextAreaProcessingCompleted>(area =>
            {
                SubscriptionsByDocumentId.TryGetValue(area.DocumentId, out var subscription);
                if (subscription == null)
                    return;
                foreach (var subscriber in subscription.Subscribers)
                {
                    subscriber.Tell(area);
                    Console.WriteLine(
                        $"Document {area.DocumentId}, page {area.PageNumber} area {area.AreaName} completed with output: " +
                        $"{area.RecognitionResult.RecognitionOutput}");
                }
            });

            Receive<DocumentPageChoiceAreaProcessingCompleted>(area =>
            {
                SubscriptionsByDocumentId.TryGetValue(area.DocumentId, out var subscription);
                if (subscription == null)
                    return;
                foreach (var subscriber in subscription.Subscribers)
                {
                    subscriber.Tell(area);
                    Console.WriteLine(
                        $"Document {area.DocumentId}, page {area.PageNumber} area {area.AreaName} completed with output: " +
                        $"{area.RecognitionResult.RecognitionOutput}");
                }
            });

        }

        protected class DocumentSubscription
        {
            public ProcessDocumentOfIdRequest Request { get; }
            public ICollection<IActorRef> Subscribers { get; }

            public DocumentSubscription(ProcessDocumentOfIdRequest request, IActorRef sender)
            {
                Request = request;
                Subscribers = new List<IActorRef>();
                Subscribers.Add(sender);
            }
        }
    }
}
