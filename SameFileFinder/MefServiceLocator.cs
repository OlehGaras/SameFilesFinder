using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Reflection;

namespace SameFileFinder
{
    public class MefServiceLocator : IServiceLocator
    {
        private CompositionContainer m_Container;

        public MefServiceLocator() : this(Assembly.GetExecutingAssembly())
        {
        }

        public MefServiceLocator(params Assembly[] assemblies)
        {
            var catalog = new AggregateCatalog();
            foreach (var assembly in assemblies)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }
            m_Container = new CompositionContainer(catalog);
            m_Container.ComposeParts(this);
        }

        public void  Register<T>(T obj)
        {
            m_Container.ComposeExportedValue<T>(obj);
        }

        public TService GetInstance<TService>()
        {
            try
            {
                return m_Container.GetExportedValue<TService>();
            }
            catch (Exception )
            {
                throw new Exception("Type was not found");
            }
        }

        public TService GetInstance<TService>(string key)
        {
            try
            {
                return m_Container.GetExportedValue<TService>(key);
            }
            catch (Exception)
            {
                throw new Exception("Type was not found");
            }
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            try
            {
                return m_Container.GetExportedValues<TService>();
            }
            catch (Exception)
            {
                throw new Exception("Type was not found");
            }
        }

    }
}
