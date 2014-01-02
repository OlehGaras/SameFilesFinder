using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Text;

namespace SameFileFinder
{
    [Export(typeof(ILogger))]

    public class Logger : ILogger
    {
        public string Path { get; private set; }
        private static readonly object m_SynchObject = new object();

        [ImportingConstructor]
        public Logger() : this(@"", "log.txt")
        {
        }

        public Logger(string pathToFolder, string fileName)
        {
            if (pathToFolder == null || fileName == null) throw new ArgumentNullException();

            pathToFolder = FixPathToFolder(pathToFolder);
            string year = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            string month = DateTime.Now.Month.ToString(CultureInfo.InvariantCulture);
            string day = DateTime.Now.Day.ToString(CultureInfo.InvariantCulture);
            try
            {
                if (!Directory.Exists(pathToFolder + year + @"\" + month + @"\" + day))
                {
                    Directory.CreateDirectory(pathToFolder + year + @"\" + month + @"\" + day);
                }
                Path = pathToFolder + year + @"\" + month + @"\" + day + @"\" + fileName;

                if (!File.Exists(Path))
                {
                    File.Create(Path).Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(Exception(e));
                Console.WriteLine("Log is written to the default file 'log.txt'! ");
                Path = year + @"\" + month + @"\" + day + @"\log.txt";
            }
        }

        public string FixPathToFolder(string pathToFolder)
        {
            if (!pathToFolder.EndsWith(@"\") && pathToFolder != "")
            {
                pathToFolder += @"\";
            }
            return pathToFolder;
        }

        public void Write(Exception e)
        {
            string message = Exception(e);
            try
            {
                Write(message);
            }
            catch (Exception exc)
            {
                Console.WriteLine(Exception(exc));
            }
        }

        public void Write(string message)
        {
            lock (m_SynchObject)
            {
                using (
                var writer = new StreamWriter(new FileStream(Path, FileMode.Append)) { AutoFlush = true })
                {
                    writer.WriteLine(DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString());
                    writer.WriteLine(message);
                }               
            }
        }

        public string Exception(Exception e)
        {
            if (e == null)
                return string.Empty;
            var message = new StringBuilder();
            message.Append(e.Message + Environment.NewLine);
            if (e.StackTrace != null)
                message.Append("StackTrace:" + Environment.NewLine + e.StackTrace + Environment.NewLine);

            var ex = e as AggregateException;
            if (ex != null)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    message.Append(Exception(innerException));
                }
            }
            message.Append(Exception(e.InnerException));
            return message.ToString();
        }

        public void Write(string format, params string[] messages)
        {
            //logger.Write(string.Format("{0},{1}","123","234","345"));
            throw new NotImplementedException();
        }
    }
}