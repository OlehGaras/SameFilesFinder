using System;
using System.IO;
using System.Threading;


namespace SameFileFinder
{
    public class Logger : ILogger
    {
        private string m_Path;
        private static readonly object m_SynchObject = new object();

        public Logger(string file_path)
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            m_Path = year + @"\" + month + @"\" + day + @"\" + file_path;
            if (!File.Exists(m_Path))
            {
                File.Create(m_Path);
            }
        }

        public void Write(Exception e)
        {
            string message = "Exception:\n" + e.Message;
            if (e.InnerException != null)
                message += Exception(e.InnerException);
            
            Write(message);
        }

        public void Write(string message)
        {
            lock (m_SynchObject)
            {
                using (
                var writer = new StreamWriter(new FileStream(m_Path, FileMode.Append)) { AutoFlush = true })
                {
                    writer.WriteLine(DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString());
                    writer.WriteLine(message);
                }
            }
        }

        public string Exception(Exception e)
        {
            string message = string.Empty;
            var ex = e as AggregateException;
            if (ex != null)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    message += Exception(innerException) + Environment.NewLine;
                    message += Environment.NewLine + "StackTrace:" + Environment.NewLine + e.StackTrace + Environment.NewLine;
                }
                return message;
            }
            return string.Empty;
        }

        public void Write(string format, params string[] messages)
        {
            //logger.Write(string.Format("{0},{1}","123","234","345"));
            throw new NotImplementedException();
        }
    }
}