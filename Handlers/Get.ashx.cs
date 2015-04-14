using System;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using SuperScript.ExternalFile.Storage;

namespace SuperScript.ExternalFile.Handlers
{
	/// <summary>
	/// Gets the specified file from the implementation of <see cref="IStore"/>.
    /// </summary>
    public class Get : IHttpHandler
    {
        // the implementation of IStore to be used for retrieving the file contents
        private readonly IStore _storeProvider;


        public RequestContext RequestContext { get; set; }


        public Get()
        {
            // verify that we've been given an implementation of IStore to use

            _storeProvider = Configuration.Settings.Instance.StoreProvider;
            if (_storeProvider == null)
            {
                throw new NotSpecifiedException("No implementation of SuperScript.ExternalFile.Storage.IStore has been specified.");
            }
        }


	    public Get(RequestContext requestContext) : this()
	    {
	        RequestContext = requestContext;
	    }


	    /// <summary>
		/// The main entry point for incoming requests.
		/// </summary>
        public void ProcessRequest(HttpContext context)
		{
	        // extract the required filename from the HTTP request

			var filename = ExtractFileName(context.Request.RawUrl);


	        // now get the instance of Storable from the storage provider

            var storable = ReadFromStorage(filename);

            if (storable == null)
            {
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }


            // handle the MIME-type for this file

            context.Response.ContentType = storable.ContentType;


			// set the Cache-control header

			if (storable.CacheForTimePeriod.Ticks > 0)
			{
				context.Response.Cache.SetCacheability(HttpCacheability.Public);
				context.Response.Cache.SetExpires(DateTime.Now.Add(storable.CacheForTimePeriod));
			}
			else
			{
				context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			}


            // should the file be deleted or left to live another day?
			if (!ShouldIgnoreLongevity(context))
			{
				switch (storable.Longevity)
				{
					case Longevity.SingleUse:
						DeleteFromStorage(filename);
						break;

					case Longevity.Reuse:
						// we don't need to do anything when we find this switch
						break;
				}
			}


			// write the contents to the output stream

            context.Response.Write(storable.Contents);
        }


        public bool IsReusable
        {
            get { return false; }
        }


        private void DeleteFromStorage(string fileName)
        {
            _storeProvider.Delete(fileName);
        }


        private string ExtractFileName(string rawUrl)
		{
			if (String.IsNullOrWhiteSpace(rawUrl))
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

            // assume that everything after the final '/' is the sought-after filename
            var idx = rawUrl.LastIndexOf('/');
            return idx == -1
                       ? rawUrl
                       : rawUrl.Substring(idx + 1);
        }


        private IStorable ReadFromStorage(string fileName)
        {
            return _storeProvider.Get(fileName);
        }


		private bool ShouldIgnoreLongevity(HttpContext context)
		{
			// the query-string is not a good place for this switch because of the possibility of 
			// the key (in the URL) containing a question-mark.
			return context.Request.RawUrl.Contains("/ignorelgvty/");
		}
    }
}