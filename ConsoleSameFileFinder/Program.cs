using SameFileFinder;


namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new ConsoleManager(args);
            manager.Execute(ServiceLocator.Current.GetInstance<IFileManager>(), ServiceLocator.Current.GetInstance<IFinder>(), ServiceLocator.Current.GetInstance<ILogger>());

            //var serviceLocator = new AutofacServiceLocator();
            //var manager = new ConsoleManager(args);
            //manager.Execute(serviceLocator.GetInstance<IFileManager>(), serviceLocator.GetInstance<IFinder>(), serviceLocator.GetInstance<ILogger>());
        }
    }
}