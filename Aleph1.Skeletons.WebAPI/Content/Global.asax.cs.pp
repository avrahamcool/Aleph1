using Aleph1.Logging;
using System.Web.Http;

namespace $rootnamespace$
{
    /// <summary>WebAPI Globals</summary>
    /// <seealso cref="System.Web.HttpApplication" />
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>Applications start.</summary>
        [Logged(LogParameters = false)]
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
