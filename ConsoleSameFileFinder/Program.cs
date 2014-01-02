using SameFileFinder;


namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            //var serviceLocator = new MefServiceLocator();
            //var manager = new ConsoleManager(args);
            //manager.Execute(serviceLocator.GetInstance<IFileManager>(), serviceLocator.GetInstance<IFinder>(), serviceLocator.GetInstance<ILogger>());

            var serviceLocator = new AutofacServiceLocator();
            var manager = new ConsoleManager(args);
            manager.Execute(serviceLocator.GetInstance<IFileManager>(), serviceLocator.GetInstance<IFinder>(), serviceLocator.GetInstance<ILogger>());
        }
    }
}