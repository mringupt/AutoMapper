using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Linq;

namespace AutoMapper
{
    public class AutoMapperInstaller : IWindsorInstaller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AutoMapperInstaller));

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
          
            container.Register(Component.For<IMapper>().UsingFactoryMethod(x =>
            {
                return new MapperConfiguration(c =>
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("Test")))
                    {
                        var profiles = from t in assembly.GetTypes()
                                       where typeof(Profile).IsAssignableFrom(t) && t.Assembly != typeof(Profile).Assembly
                                       select Activator.CreateInstance(t) as Profile;

                        foreach (var profile in profiles)
                        {
                            if (_log.IsInfoEnabled)
                            {
                                _log.InfoFormat("Registering Profile: {0}", profile.GetType().FullName);
                            }

                            c.AddProfile(profile);
                        }
                    }
                }).CreateMapper();
            }));
            
        }
    }
}
