using System.Security.Principal;
using System.Web;

namespace Aleph1.Utitilies
{
    /// <summary>Handles common User related tasks (such as, getting the current user logon name)</summary>
    public static class UserExtentions
    {
        /// <summary>Get the current user logon name</summary>
        public static string CurrentUserName
        {
            get
            {
                return HttpContext.Current?.User?.Identity?.Name ?? WindowsIdentity.GetCurrent()?.Name ?? string.Empty;
            }
        }
    }
}
