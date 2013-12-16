using System;
using System.IO;


namespace SameFileFinder
{
    public class Logger : ILogger
    {
        private string m_path;
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
            m_path = year + @"\" + month + @"\" + day + @"\" + file_path;

        }

        public void AppStart()
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(m_path, FileMode.OpenOrCreate)) { AutoFlush = true })
            {
                writer.WriteLine("\t APPLICATION STARTED");
                writer.WriteLine(DateTime.Now.ToLongDateString());
                writer.WriteLine("Application started at " + DateTime.Now.ToLongTimeString());
            }
        }

        public void WriteToFile(Exception e)
        {
            AppAbort();
            string message = "Exception:\n" + e.Message;
            if (e.InnerException != null)
                message += Environment.NewLine + "InnerException:\n" + e.InnerException.ToString();
            if (e.StackTrace != null)
                message += Environment.NewLine + "StackTrace:\n" + e.StackTrace.ToString();

            using (StreamWriter writer = new StreamWriter(new FileStream(m_path, FileMode.Append)) { AutoFlush = true })
            {
                writer.WriteLine(message);
            }
        }

        public void AppAbort()
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(m_path, FileMode.Append)) { AutoFlush = true })
            {
                writer.WriteLine(DateTime.Now.ToLongDateString());
                writer.WriteLine("Application Warning at" + DateTime.Now.ToLongTimeString());
            }
        }

        public void AppEnd()
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(m_path, FileMode.Append)) { AutoFlush = true })
            {
                writer.WriteLine("\t APPLICATION END");
                writer.WriteLine(DateTime.Now.ToLongDateString());
                writer.WriteLine("Application end at " + DateTime.Now.ToLongTimeString());
            }
        }

        public void Write(string format, params string[] messages)
        {
            //logger.Write(string.Format("{0},{1}","123","234","345"));
            throw new NotImplementedException();
        }
    }
}