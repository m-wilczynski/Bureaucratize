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
using Akka.DI.AutoFac;
using Autofac;
using Bureaucratize.ImageProcessing.Host.IoC.Modules;

namespace Bureaucratize.ImageProcessing.Host.IoC
{
    public static class ActorSystemIocExtensions
    {
        public static ActorSystem WithIocContainer(this ActorSystem system)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<PersistenceDependenciesModule>();
            builder.RegisterModule<ImageProcessingDependenciesModule>();
            builder.RegisterModule<ActorDependenciesModule>();

            var container = builder.Build();
            new AutoFacDependencyResolver(container, system);

            return system;
        }
    }
}
