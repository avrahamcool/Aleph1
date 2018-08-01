using System;
using System.Security.Principal;
using System.Web;

namespace Aleph1.Utitilies
{
    /// <summary>Handles common User related tasks (such as, getting the current user logon name)</summary>
    public static class UserExtentions
    {
        /// <summary>Get the current user logon name</summary>
        /// <remarks>1) Identity from HttpContext: Name => IP => Empty string, 2) Identity from Windows Context</remarks>
        public static string CurrentUserName
        {
            get
            {
                if(HttpContext.Current != null)
                {
                    string name = HttpContext.Current?.User?.Identity?.Name;
                    return String.IsNullOrWhiteSpace(name) ? (HttpContext.Current?.Request?.UserHostAddress ?? String.Empty) : name;
                }
                return WindowsIdentity.GetCurrent()?.Name ?? String.Empty;
            }
        }
    }
}
