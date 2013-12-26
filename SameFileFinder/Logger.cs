using System;
using System.IO;
using System.Text;
using System.Threading;


namespace SameFileFinder
{
    public class Logger : ILogger
    {
        private string m_Path;
        private static readonly object m_SynchObject = new object();

        public Logger(string filePath)
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            if (!Directory.Exists(year + @"\" + month + @"\" + day))
            {
                Directory.CreateDirectory(year + @"\" + month + @"\" + day);
            }
            m_Path = year + @"\" + month + @"\" + day + @"\" + filePath;
            if (!File.Exists(m_Path))
            {
                File.Create(m_Path);
            }
        }

        public void Write(Exception e)
        {
            string message = Exception(e);
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
                    message.Append( Exception(innerException));
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