using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Lib;

namespace WingtipToys.Logic
{
  public class LogFileAccess : ILogFileAccess
  {
    public string LogFileName
    {
      get {
        string logFile = "App_Data/ErrorLog.txt";
        return HttpContext.Current.Server.MapPath(logFile);
      }
    }
  }
}
