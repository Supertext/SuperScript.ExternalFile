using System.Web;
using System.Web.Routing;

namespace SuperScript.ExternalFile.Handlers.Routing
{
    public class Get : IRouteHandler
    {
        /// <summary>
        /// Handles the routing for <c>/files/{filename}</c> requests.
        /// </summary>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new Handlers.Get(requestContext);
        }
    }
}