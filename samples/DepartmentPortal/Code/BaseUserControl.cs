using System;
using System.Web;
using System.Web.UI;

namespace DepartmentPortal
{
    public class BaseUserControl : UserControl
    {
        public string ControlId { get; set; }

        protected void LogActivity(string activity)
        {
            System.Diagnostics.Debug.WriteLine(
                string.Format("[{0}] Control '{1}': {2}",
                    DateTime.Now.ToString("HH:mm:ss"), ControlId ?? ID, activity));
        }

        protected T CacheGet<T>(string key)
        {
            object cached = HttpRuntime.Cache[key];
            if (cached is T)
            {
                return (T)cached;
            }
            return default(T);
        }

        protected void CacheSet<T>(string key, T value, int minutes = 10)
        {
            HttpRuntime.Cache.Insert(key, value, null,
                DateTime.Now.AddMinutes(minutes),
                System.Web.Caching.Cache.NoSlidingExpiration);
        }
    }
}
