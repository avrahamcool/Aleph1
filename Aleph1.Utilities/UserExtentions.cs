using System.Security.Principal;
using System.Web;

namespace Aleph1.Utilities
{
	/// <summary>Handles common User related tasks (such as, getting the current user login name)</summary>
	public static class UserExtentions
	{
		/// <summary>Get the current user login name</summary>
		/// <remarks>1) Identity from HttpContext: Name => IP => Empty string, 2) Identity from Windows Context</remarks>
		public static string CurrentUserName
		{
			get
			{
				// Accessing HttpContext.Current.Request Throws Exception when no handler configured
				if (HttpContext.Current != null && HttpContext.Current.Handler != null)
				{
					string identifierFromHttp = string.IsNullOrWhiteSpace(HttpContext.Current.User?.Identity?.Name) ?
						HttpContext.Current.Request.UserHostAddress :
						HttpContext.Current.User.Identity.Name;
					return identifierFromHttp ?? string.Empty;
				}

				return WindowsIdentity.GetCurrent()?.Name ?? string.Empty;
			}
		}
	}
}
