using System;
using System.IO;


namespace SameFileFinder
{
    public class Logger : ILogger
    {
        private string m_Path;
        public Logger(string file_path)
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            if (!Directory.Exists(year))
            {
                Directory.CreateDirectory(year);
            }
            if (!Directory.Exists(year + @"\" + month))
            {
                Directory.CreateDirectory(year + @"\" + month);
            }
            if (!Directory.Exists(year + @"\" + month + @"\" + day))
            {
                Directory.CreateDirectory(year + @"\" + month + @"\" + day);
            }
            m_Path = year + @"\" + month + @"\" + day + @"\" + file_path;

        }

        public void Write(Exception e)
        {
            string message = "Exception:\n" + e.Message;
            if (e.InnerException != null)
                message += Environment.NewLine + "InnerException:\n" + e.InnerException.ToString();
            if (e.StackTrace != null)
                message += Environment.NewLine + "StackTrace:\n" + e.StackTrace.ToString();
            Write(message);
        }

        public void Write(string message)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(m_Path, FileMode.Append)) { AutoFlush = true })
            {
                writer.WriteLine(DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString());
                writer.WriteLine(message);
            }
        }

        public void Write(string format, params string[] messages)
        {
            //logger.Write(string.Format("{0},{1}","123","234","345"));
            throw new NotImplementedException();
        }
    }
}