using System.Web;
using System.Web.Routing;

namespace SuperScript.ExternalFile.Handlers.Routing
{
    public class Empty : IRouteHandler
    {
        /// <summary>
        /// Handles the routing for <c>/empty/</c> requests.
        /// </summary>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new Handlers.Empty(requestContext);
        }
    }
}