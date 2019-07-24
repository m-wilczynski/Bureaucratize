using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.Configuration;

namespace Bureaucratize.Web.Config
{
    public static class SetupAkka
    {
        public static string RemoteActorSystemAddress(IConfiguration configuration) => 
            $"{configuration["ProcessingHost:Address"]}:{configuration["ProcessingHost:port"]}";

        public static void BootstrapActorSystem(IConfiguration configuration)
        {
            DocumentSystemActors.Config = configuration;
            var config = ConfigurationFactory.ParseString(File.ReadAllText("web.hocon"));
            DocumentSystemActors.ActorSystem = ActorSystem.Create("web", config);
            try
            {
                DocumentSystemActors.RemoteImageProcessing = DocumentSystemActors.ActorSystem
                    .ActorSelection($"akka.tcp://api@{RemoteActorSystemAddress(configuration)}/user/image-processing")
                    .ResolveOne(TimeSpan.FromMilliseconds(3000)).Result;
            }
            catch
            {
                DocumentSystemActors.RemoteImageProcessing = null;
            }
        }
    }

    public static class DocumentSystemActors
    {
        private static IActorRef _remoteRef = null;

        public static ActorSystem ActorSystem { get; set; }
        public static IConfiguration Config { get; set; }
        public static IActorRef RemoteImageProcessing
        {
            get
            {
                if (_remoteRef != null)
                {
                    return _remoteRef;
                }

                try
                {
                    return ActorSystem
                        .ActorSelection(
                            $"akka.tcp://{SetupAkka.RemoteActorSystemAddress(Config)}/user/image-processing")
                        .ResolveOne(TimeSpan.FromMilliseconds(3000)).Result;
                }
                catch
                {
                    return null;
                }
            }
            set => _remoteRef = value;
        }
    }
}
