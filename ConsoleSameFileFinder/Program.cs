using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using SameFileFinder;


namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger(@"", "log.txt");
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Finder).Assembly));
            var container = new CompositionContainer(catalog);
            container.ComposeExportedValue<ILogger>(logger);
            var manager = new ConsoleManager(args, container.GetExportedValue<ILogger>());
            manager.Execute(container.GetExportedValue<IFileManager>(), container.GetExportedValue<IFinder>(), container.GetExportedValue<ILogger>());
        }
    }
}
