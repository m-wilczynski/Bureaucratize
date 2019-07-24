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

using Akka.Actor;
using Bureaucratize.ImageProcessing.Host.Actors.ImageProcessing;
using Bureaucratize.ImageProcessing.Host.IoC;

namespace Bureaucratize.ImageProcessing.Host
{
    public class ActorSystemService
    {
        private ActorSystem _actorSystem;
        private IActorRef _processingMaster;
        private IActorRef _coordinationMaster;

        public void Start()
        {
            _actorSystem = ActorSystem.Create("api").WithIocContainer();
            _processingMaster = _actorSystem.ActorOf<DocumentImageProcessingMaster>("image-processing");
        }

        public async void Stop()
        {
            await _actorSystem.Terminate();
        }
    }
}
