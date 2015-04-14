using System;
using System.Net;
using System.Web;
using System.Web.Routing;
using SuperScript.ExternalFile.Storage;

namespace SuperScript.ExternalFile.Handlers
{
    /// <summary>
    /// <para>Causes a request to be made to the <see cref="IStore"/> implementation to empty the store.</para>
    /// <para>Note that this request does not remove the store.</para>
    /// </summary>
    /// <seealso cref="Remove"/>
    public class Empty : IHttpHandler
    {
        // the implementation of IStore to be used for processing the call
        private readonly IStore _storeProvider;


        public RequestContext RequestContext { get; set; }


        /// <summary>
        /// <para>Default constructor for <see cref="Empty"/>.</para>
        /// </summary>
        public Empty()
        {
            // verify that we've been given an implementation of IStore to use

            _storeProvider = Configuration.Settings.Instance.StoreProvider;
            if (_storeProvider == null)
            {
                throw new NotSpecifiedException("No implementation of SuperScript.ExternalFile.Storage.IStore has been specified.");
            }
        }


        public Empty(RequestContext requestContext) : this()
        {
            RequestContext = requestContext;
        }


        /// <summary>
        /// The main entry point for incoming requests.
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetExpires(DateTime.MinValue);

            _storeProvider.Empty();
            context.Response.StatusCode = (int) HttpStatusCode.OK;
        }


        public bool IsReusable
        {
            get { return false; }
        }
    }
}