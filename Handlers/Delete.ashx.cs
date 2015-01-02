using System;
using System.Net;
using System.Web;
using System.Web.Http;
using SuperScript.ExternalFile.Storage;

namespace SuperScript.ExternalFile.Handlers
{
	/// <summary>
	/// Causes a request to be made to the <see cref="IStore"/> implementation to delete a specified file from the store.
	/// </summary>
	/// <seealso cref="Remove"/>
	public class Delete : IHttpHandler
	{
		// the implementation of IStore to be used for processing the call
		private readonly IStore _storeProvider;


		/// <summary>
		/// The main entry point for incoming requests.
		/// </summary>
		public void ProcessRequest(HttpContext context)
		{
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Cache.SetNoStore();
			context.Response.Cache.SetExpires(DateTime.MinValue);

			// extract the required filename from the HTTP request
			var filename = ExtractFileName(context.Request.RawUrl);

			_storeProvider.Delete(filename);
			context.Response.StatusCode = (int) HttpStatusCode.OK;
		}


		public bool IsReusable
		{
			get { return false; }
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


		/// <summary>
		/// <para>Default constructor for <see cref="Delete"/>.</para>
		/// </summary>
		public Delete()
		{
			// verify that we've been given an implementation of IStore to use

			_storeProvider = Configuration.Settings.Instance.StoreProvider;
			if (_storeProvider == null)
			{
				throw new NotSpecifiedException("No implementation of SuperScript.ExternalFile.Storage.IStore has been specified.");
			}
		}
	}
}