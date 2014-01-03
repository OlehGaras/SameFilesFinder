using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace SameFileFinder
{
    public class AutofacServiceLocator : IServiceLocator
    {
        private ContainerBuilder m_Builder;
        private IContainer m_Container;

        public AutofacServiceLocator()
            : this(Assembly.GetExecutingAssembly())
        {
        }

        public AutofacServiceLocator(params Assembly[] assemblies)
        {
            m_Builder = new ContainerBuilder();
            m_Builder.RegisterAssemblyTypes(assemblies).AsImplementedInterfaces();
            m_Container = m_Builder.Build();
        }

        public TService GetInstance<TService>()
        {
            if (m_Container.IsRegistered<TService>())
                return m_Container.Resolve<TService>();
            return default(TService);
        }

        public TService GetInstance<TService>(string key)
        {
            if (m_Container.IsRegistered<TService>())
                return m_Container.ResolveNamed<TService>(key);
            return default(TService);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            if (m_Container.IsRegistered<TService>())
                return m_Container.Resolve<IEnumerable<TService>>();
            return default(IEnumerable<TService>);
        }

        public void Register<T>(T obj)
        {
            m_Builder.RegisterType(obj.GetType());
            m_Container = m_Builder.Build();
        }
    }
}
