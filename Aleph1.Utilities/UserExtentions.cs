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
                // Accessing HttpContext.Current.Request Throws Exception when no handler configured
                if (HttpContext.Current != null && HttpContext.Current.Handler != null)
                    return String.IsNullOrWhiteSpace(HttpContext.Current.User?.Identity?.Name) ?
                        HttpContext.Current.Request.UserHostAddress :
                        HttpContext.Current.User.Identity.Name
                        ?? String.Empty;

                return WindowsIdentity.GetCurrent()?.Name ?? String.Empty;
            }
        }
    }
}
