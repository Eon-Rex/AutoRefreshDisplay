using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AmberProject.LogContent
{
    public class WriteLogs
    { public static void WriteLog(string WriteMessage, string FileName)
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles"))
                { Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles"); }
                using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\" + FileName, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss ::->") + WriteMessage);
                }
            }
            catch { }
            finally { }
        }
    }
}