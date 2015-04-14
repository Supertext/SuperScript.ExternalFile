using System.Web;
using System.Web.Routing;

namespace SuperScript.ExternalFile.Handlers.Routing
{
    public class Delete : IRouteHandler
    {
        /// <summary>
        /// Handles the routing for <c>/delete/{filename}</c> requests.
        /// </summary>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new Handlers.Delete(requestContext);
        }
    }
}