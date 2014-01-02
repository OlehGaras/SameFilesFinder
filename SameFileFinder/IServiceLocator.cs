using System;
using System.Collections.Generic;

namespace SameFileFinder
{
    public  interface IServiceLocator
    {
        TService GetInstance<TService>();
        TService GetInstance<TService>(string key);
        IEnumerable<TService> GetAllInstances<TService>();
        void Register<T>(T obj);
    }
}
