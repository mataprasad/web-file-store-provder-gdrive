using Autofac;

namespace FileUploadProvider.LocalDirectory
{
    public class DI : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LocalDirectory>().As<IUploadProvider>();
        }
    }
}
