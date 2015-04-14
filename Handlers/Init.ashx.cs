﻿using System;
using System.Net;
using System.Web;
using System.Web.Routing;
using SuperScript.ExternalFile.Storage;

namespace SuperScript.ExternalFile.Handlers
{
    /// <summary>
    /// Causes a request to be made to the <see cref="IStore"/> implementation to initialise the store on this machine.
    /// </summary>
    public class Init : IHttpHandler
    {
        // the implementation of IStore to be used for processing the call
        private readonly IStore _storeProvider;


        public RequestContext RequestContext { get; set; }


        /// <summary>
        /// <para>Default constructor for <see cref="Init"/>.</para>
        /// </summary>
        public Init()
        {
            // verify that we've been given an implementation of IStore to use

            _storeProvider = Configuration.Settings.Instance.StoreProvider;
            if (_storeProvider == null)
            {
                throw new NotSpecifiedException("No implementation of SuperScript.ExternalFile.Storage.IStore has been specified.");
            }
        }


        public Init(RequestContext requestContext) : this()
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

            _storeProvider.Init();
            context.Response.StatusCode = (int) HttpStatusCode.OK;
        }


        public bool IsReusable
        {
            get { return false; }
        }
    }
}