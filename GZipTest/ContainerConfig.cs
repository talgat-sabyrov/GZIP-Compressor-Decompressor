using Autofac;
using GZipTest.Abstractions;
using GZipTest.Business;

namespace GZipTest
{
    //IOC container
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<QueueReader>().As<IQueueReader>();
            builder.RegisterType<QueueWriter>().As<IQueueWriter>();
            builder.RegisterType<Validator>().As<IValidator>();
            builder.Register(c => new GZipProgress()).As<IGZipProgress>().SingleInstance();
            builder.RegisterType<GZipProcessor>().As<IGZipProcessor>();
            
            return builder.Build();
        }
    }
}
