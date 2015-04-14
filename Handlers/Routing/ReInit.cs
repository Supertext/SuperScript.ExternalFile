using System.Web;
using System.Web.Routing;

namespace SuperScript.ExternalFile.Handlers.Routing
{
    public class ReInit : IRouteHandler
    {
        /// <summary>
        /// Handles the routing for <c>/delete/{filename}</c> requests.
        /// </summary>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new Handlers.ReInit(requestContext);
        }
    }
}