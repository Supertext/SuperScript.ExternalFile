using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Xml.Linq;
using SuperScript.ExternalFile.Storage;

namespace SuperScript.ExternalFile.Handlers
{
	/// <summary>
	/// Returns a webpage enumerating all instances of <see cref="IStorable"/>.
	/// </summary>
	public class List : IHttpHandler
	{
		// the implementation of IStore to be used for processing the call
        private readonly IStore _storeProvider;


        public RequestContext RequestContext { get; set; }


        /// <summary>
        /// <para>Default constructor for <see cref="List"/>.</para>
        /// </summary>
        public List()
        {
            // verify that we've been given an implementation of IStore to use

            _storeProvider = Configuration.Settings.Instance.StoreProvider;
            if (_storeProvider == null)
            {
                throw new NotSpecifiedException("No implementation of SuperScript.ExternalFile.Storage.IStore has been specified.");
            }
        }


        public List(RequestContext requestContext) : this()
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
			context.Response.ContentType = "text/html; charset=utf-8";

			var storables = _storeProvider.GetAll();

			var doc = new XDocument();
			doc.AddFirst(new XDocumentType("html", null, null, null));
			doc.Add(new XElement("html",
			                     new XElement("head",
			                                  new XElement("meta",
			                                               new XAttribute("charset", "utf-8")),
			                                  new XElement("meta",
			                                               new XAttribute("name", "viewport"),
			                                               new XAttribute("content", "width=device-width, initial-scale=1.0")),
			                                  new XElement("title", Resources.List.Name + " -" + Resources.List.PageTitle),
			                                  new XElement("style", BuildStyles)),
			                     new XElement("body",
			                                  new XElement("h1", Resources.List.Name),
			                                  new XElement("h2", Resources.List.PageTitle),
			                                  new XElement("p", Resources.List.PageHeader),
			                                  new XElement("p",
			                                               new XElement("span",
			                                                            new XAttribute("id", "reload"),
			                                                            new XAttribute("class", "instruction"),
			                                                            Resources.List.InstructionReload)),
			                                  new XElement("table",
			                                               new XAttribute("id", "storables"),
			                                               new XElement("thead",
			                                                            new XElement("tr",
			                                                                         new XElement("th", Resources.List.HeaderKey),
			                                                                         new XElement("th", Resources.List.HeaderContentLength),
			                                                                         new XElement("th", Resources.List.HeaderContentType),
			                                                                         new XElement("th", Resources.List.HeaderLongevity),
			                                                                         new XElement("th", Resources.List.HeaderClientCachePeriod),
			                                                                         new XElement("th", new XRaw("&nbsp;")),
			                                                                         new XElement("th", new XRaw("&nbsp;")))),
			                                               new XElement("tbody",
			                                                            from storable in storables
			                                                            select new XElement("tr",
			                                                                                new XElement("td",
			                                                                                             new XElement("span",
			                                                                                                          new XAttribute("class", "slide-up"),
			                                                                                                          storable.Key)),
			                                                                                new XElement("td",
			                                                                                             new XElement("span",
			                                                                                                          new XAttribute("class", "slide-up"),
			                                                                                                          storable.Contents.Length)),
			                                                                                new XElement("td",
			                                                                                             new XElement("span",
			                                                                                                          new XAttribute("class", "slide-up"),
			                                                                                                          storable.ContentType)),
			                                                                                new XElement("td",
			                                                                                             new XElement("span",
			                                                                                                          new XAttribute("class", "slide-up"),
			                                                                                                          Enum.GetName(typeof (Longevity), storable.Longevity))),
			                                                                                new XElement("td",
			                                                                                             new XElement("span",
			                                                                                                          new XAttribute("class", "slide-up"),
			                                                                                                          storable.CacheForTimePeriod.ToString("g"))),
			                                                                                new XElement("td",
			                                                                                             new XElement("a",
			                                                                                                          new XAttribute("class", "slide-up"),
																													  new XAttribute("href", "/" + Configuration.Settings.Instance.HandlerMappings.Get + "/ignorelgvty/" + storable.Key),
			                                                                                                          new XAttribute("target", "_new"),
																													  new XAttribute("title", Resources.List.TooltipView),
			                                                                                                          Resources.List.InstructionView)),
			                                                                                new XElement("td",
			                                                                                             new XElement("span",
			                                                                                                          new XAttribute("class", "instruction delete slide-up"),
			                                                                                                          new XAttribute("rel", storable.Key),
			                                                                                                          Resources.List.InstructionDelete))))),
			                                  new XElement("p",
			                                               new XElement("span",
			                                                            new XAttribute("id", "empty"),
			                                                            new XAttribute("class", "instruction"),
			                                                            Resources.List.InstructionEmpty),
			                                               new XElement("span",
			                                                            new XAttribute("id", "empty-msg"),
			                                                            new XAttribute("class", "margin-l-2em hidden"),
			                                                            Resources.List.MsgEmpty)),
			                                  new XElement("p",
			                                               new XElement("span",
			                                                            new XAttribute("id", "init"),
			                                                            new XAttribute("class", "instruction"),
			                                                            Resources.List.InstructionInit),
			                                               new XElement("span",
			                                                            new XAttribute("id", "init-msg"),
			                                                            new XAttribute("class", "margin-l-2em hidden"),
			                                                            Resources.List.MsgInit)),
			                                  new XElement("p",
			                                               new XElement("span",
			                                                            new XAttribute("id", "reinit"),
			                                                            new XAttribute("class", "instruction"),
			                                                            Resources.List.InstructionReInit),
			                                               new XElement("span",
			                                                            new XAttribute("id", "reinit-msg"),
			                                                            new XAttribute("class", "margin-l-2em hidden"),
			                                                            Resources.List.MsgReInit)),
			                                  new XElement("p",
			                                               new XElement("span",
			                                                            new XAttribute("id", "remove"),
			                                                            new XAttribute("class", "instruction"),
			                                                            Resources.List.InstructionRemove),
			                                               new XElement("span",
			                                                            new XAttribute("id", "remove-msg"),
			                                                            new XAttribute("class", "margin-l-2em hidden"),
			                                                            Resources.List.MsgRemove)),
			                                  new XElement("p",
			                                               new XAttribute("id", "error-msg"),
			                                               new XAttribute("class", "hidden")),
			                                  new XElement("script",
			                                               new XAttribute("type", "text/javascript"),
			                                               new XAttribute("src", "http://code.jquery.com/jquery-latest.min.js"),
			                                               String.Empty),
			                                  new XElement("script",
			                                               new XAttribute("type", "text/javascript"),
			                                               BuildJavaScript))));

			context.Response.Write(doc);
		}


		public bool IsReusable
		{
			get { return false; }
		}


		private XRaw BuildJavaScript
		{
			get { return new XRaw(@"var externalFile = function($, win, doc) {

								var elmntErrorMsg = $(""#error-msg""),
									mapDelete = ""delete"",
									mapEmpty = ""empty"",
									mapInit = ""init"",
									mapList = ""list"",
									mapReinit = ""reinit"",
									mapRemove = ""remove"";

								function makeRequest(req, objData, urlAdditions, callbackSuccess, callbackError, callbackAlways) {

									if (!req) {
										return;
									}

									// if an error-msg is showing, hide it
									if (!elmntErrorMsg.hasClass(""hidden"")) {
										elmntErrorMsg.addClass(""hidden"");
									}

									$.ajax({
										data: objData,
										type: ""GET"",
										url: ""/"" + req + ""/"" + urlAdditions,
										success: function(data, textStatus, jqXhr) {
											if (typeof callbackSuccess === ""function"") {
												callbackSuccess(data, textStatus, jqXhr);
											}
										},
										error: function(jqXHR, textStatus, errorThrown) {
											if (typeof callbackError === ""function"") {
												callbackError(jqXHR, textStatus, errorThrown);
											}
										},
										complete: function(jqXHR, textStatus) {
											if (typeof callbackAlways === ""function"") {
												callbackAlways(jqXHR, textStatus);
											}
										}
									});
								}

								function removeAllEntries() {
									var elmntTRs = $(""tbody > tr"");
									elmntTRs.find("".slide-up"").each(function() {
										$(this).slideUp(function() {
											elmntTRs.remove();
										});
									});
								}

								function showErrorMsg(jqXHR, textStatus, errorThrown) {
									elmntErrorMsg.html(errorThrown).removeClass(""hidden"");
								}

								return {
									Init: function(options) {
										$(""body"").on(""click"", "".instruction"", function() {

											var self = this,
												fnAlways = null,
												elmnt = $(self),
												req;

											if (elmnt.hasClass(""delete"")) {
												var key = $(self).attr(""rel"");
												var fnSuccess = function(elmntDelete) {
													var elmntDelete = $(self).parents(""tr"");
													if (elmntDelete.length) {
														elmntDelete.find("".slide-up"").each(function() {
															$(this).slideUp(function() {
																elmntDelete.remove();
															});
														});
													}
												};
												makeRequest(mapDelete, null, key, fnSuccess, showErrorMsg, fnAlways);
											}
										});

										$(""#empty"").click(function() {
											if (confirm(""" + Resources.List.ConfirmEmpty + @""")) {
												var elmntMsg = $(""#empty-msg""),
													fnAlways = null,
													fnSuccess = function() {
														elmntMsg.removeClass(""hidden"");
														removeAllEntries();
													};
													elmntMsg.addClass(""hidden"");
												makeRequest(mapEmpty, null, """", fnSuccess, showErrorMsg, fnAlways);
											}
										});

										$(""#init"").click(function() {
											if (confirm(""" + Resources.List.ConfirmInit + @""")) {
												var elmntMsg = $(""#init-msg""),
													fnAlways = null,
													fnSuccess = function() {
														elmntMsg.removeClass(""hidden"");
														removeAllEntries();
													};
													elmntMsg.addClass(""hidden"");
												makeRequest(mapInit, null, """", fnSuccess, showErrorMsg, fnAlways);
											}
										});

										$(""#reinit"").click(function() {
											if (confirm(""" + Resources.List.ConfirmReinit + @""")) {
												var elmntMsg = $(""#reinit-msg""),
													fnAlways = null,
													fnSuccess = function() {
														elmntMsg.removeClass(""hidden"");
														removeAllEntries();
													};
													elmntMsg.addClass(""hidden"");
												makeRequest(mapReinit, null, """", fnSuccess, showErrorMsg, fnAlways);
											}
										});

										$(""#reload"").click(function() {
											win.location.reload();
										});

										$(""#remove"").click(function() {
											if (confirm(""" + Resources.List.ConfirmRemove + @""")) {
												var elmntMsg = $(""#remove-msg""),
													fnAlways = null,
													fnSuccess = function() {
														elmntMsg.removeClass(""hidden"");
														removeAllEntries();
													};
													elmntMsg.addClass(""hidden"");
												makeRequest(mapRemove, null, """", fnSuccess, showErrorMsg, fnAlways);
											}
										});

										$(""table tbody tr:even"").addClass(""even"");
									},

									MapDelete: function(value) {
										if (typeof value === ""string"") {
											mapDelete = value;
										}

										return mapDelete;
									},

									MapEmpty: function(value) {
										if (typeof value === ""string"") {
											mapEmpty = value;
										}

										return mapEmpty;
									},

									MapInit: function(value) {
										if (typeof value === ""string"") {
											mapInit = value;
										}

										return mapInit;
									},

									MapList: function(value) {
										if (typeof value === ""string"") {
											mapList = value;
										}

										return mapList;
									},

									MapReinit: function(value) {
										if (typeof value === ""string"") {
											mapReinit = value;
										}

										return mapReinit;
									},

									MapRemove: function(value) {
										if (typeof value === ""string"") {
											mapRemove = value;
										}

										return mapRemove;
									}
								};
							}(jQuery, window, document);

							externalFile.MapDelete(""" + Configuration.Settings.Instance.HandlerMappings.Delete + @""");
							externalFile.MapEmpty(""" + Configuration.Settings.Instance.HandlerMappings.Empty + @""");
							externalFile.MapInit(""" + Configuration.Settings.Instance.HandlerMappings.Init + @""");
							externalFile.MapList(""" + Configuration.Settings.Instance.HandlerMappings.List + @""");
							externalFile.MapReinit(""" + Configuration.Settings.Instance.HandlerMappings.ReInit + @""");
							externalFile.MapRemove(""" + Configuration.Settings.Instance.HandlerMappings.Remove + @""");
							externalFile.Init();"); }
		}


		private string BuildStyles
		{
			get { return @"a {
								text-decoration: none;
								transition: color 0.5s ease;
							}
							a:hover {
								color: black;
							}
							a:visited {
								color: blue;
							}
							body {
								font-family: sans-serif;
								font-size: 14px;
							}
							span.instruction {
								color: blue;
								cursor: pointer;
								transition: color 0.5s ease;
							}
							span.instruction:hover {
								color: black;
							}
							table {
								border: none;
								border-collapse: collapse;
							}
							table thead {
								background-color: grey;
								color: white;
								font-weight: bold;
							}
							table th {
								padding: 0.25em;
								text-align: left;
							}
							table td {
								padding: 0.2em;
							}
							table tr.even {
								background-color: #eee;
							}
							#error-msg {
								color: red;
								font-weight: bold;
							}
							.hidden {
								display: none;
							}
							.margin-l-2em {
								margin-left: 2em;
							}";
			}
		}
	}


	public class XRaw : XText
	{
		public XRaw(string text) : base(text)
		{
		}

		public XRaw(XText text) : base(text)
		{
		}

		public override void WriteTo(System.Xml.XmlWriter writer)
		{
			writer.WriteRaw(Value);
		}
	}
}