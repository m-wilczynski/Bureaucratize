using System;
using Topshelf;

namespace Bureaucratize.ImageProcessing.Host
{
    public class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ActorSystemService>(s =>
                {
                    s.ConstructUsing(n => new ActorSystemService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                x.RunAsLocalSystem();
                x.UseAssemblyInfoForServiceInfo();
            });

            //var service = new ActorSystemService();
            //service.Start();

            //Console.ReadKey();
        }
    }
}
