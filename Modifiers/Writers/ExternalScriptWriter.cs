using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SuperScript.ExternalFile.Storage;
using SuperScript.ExternalFile.UrlHelpers;
using SuperScript.Modifiers;
using SuperScript.Modifiers.Post;
using SuperScript.Modifiers.Writers;

namespace SuperScript.ExternalFile.Modifiers.Writers
{
    /// <summary>
    /// <para>Writes the formatted output of the <see cref="PostModifierArgs.Emitted"/> to a storage provider, then.</para>
    /// <para>references the contents in an HTML tag.</para>
    /// <para>An instance of this class will be processed after any implementations of the abstract class <see cref="CollectionPostModifier"/>.</para>
    /// </summary>
    public class ExternalScriptWriter : HtmlWriter
    {
        //  the implementation of IStore to be used for storing the file contents.
        private IStore _storeProvider;


		#region Default values

		private TimeSpan _cacheForTimePeriod = new TimeSpan(0, 0, 0, 0);
		private string _contentType = "text/javascript";
		private string _fileExtension = "js";
		private string _handlerPath = "/" + Configuration.Settings.Instance.HandlerMappings.Get + "/";
		private Longevity _longevity = Longevity.SingleUse;
		private bool _makeUnique = true;
		private string _pathAttribute = "src";
		private bool _selfCloseTag = false;
	    private IEnumerable<KeyValuePair<string, string>> _tagAttributes = new Collection<KeyValuePair<string, string>>
		                                                                       {
			                                                                       new KeyValuePair<string, string>("type", "text/javascript")
		                                                                       };

		private string _tagName = "script";

		#endregion


		#region Properties
		
		/// <summary>
		/// <para>The period for which the client should cache the file.</para>
		/// <para>If set to null then the Cache-control header will be set to no-cache.</para>
		/// <para>The default value is 3650 days, Cache-control: public.</para>
		/// </summary>
		public TimeSpan CacheForTimePeriod
		{
			get { return _cacheForTimePeriod; }
			set { _cacheForTimePeriod = value; }
		}


		/// <summary>
		/// <para>Gets or sets the MIME type that the external file will be despatched with.</para>
		/// <para>Default is <c>text/javascript</c>.</para>
		/// </summary>
		public string ContentType
		{
			get { return _contentType; }
			set { _contentType = value; }
		}


		/// <summary>
		/// <para>Gets or sets the output file extension.</para>
		/// <para>Default value is <c>js</c>.</para>
		/// </summary>
		public string FileExtension
		{
			get { return _fileExtension; }
			set { _fileExtension = value; }
		}


		/// <summary>
		/// <para>Gets or sets the output file name.</para>
		/// <para>Default value is a hash of the contents.</para>
		/// </summary>
		public string FileName { get; set; }


		/// <summary>
		/// <para>Gets or sets the path that will be used in the URL (excluding the filename).</para>
		/// <para>This must match the 'path' attribute assigned to the handler in the web.config.</para>
		/// <para>Default is <c>/Files/</c>.</para>
		/// </summary>
		public string HandlerPath
		{
			get { return _handlerPath; }
			set
			{
				value = value.Trim(new[] { '\\' });
				if (!value.StartsWith("/"))
				{
					value = "/" + value;
				}
				if (!value.EndsWith("/"))
				{
					value = value + "/";
				}
				_handlerPath = value;
			}
		}


		/// <summary>
		/// <para>Determines the lifetime of the file.</para>
		/// <para>Default is <see cref="SuperScript.Longevity.SingleUse"/>.</para>
		/// </summary>
		public Longevity Longevity
		{
			get { return _longevity; }
			set { _longevity = value; }
		}


		/// <summary>
		/// <para>Gets or sets whether a hash of the contents should be appended as a query-string.</para>
		/// <para>If no <see cref="FileName"/> is specified then a hash will be used for the <see cref="FileName"/>.</para>
		/// <para>If the hash is used as the <see cref="FileName"/> then it will not be appended as a query-string.</para>
		/// <para>Default is <c>true</c>.</para>
		/// </summary>
		public bool MakeUnique
		{
			get { return _makeUnique; }
			set { _makeUnique = value; }
		}


		/// <summary>
		/// <para>Gets or sets the attribute which references the external file.</para>
		/// <para>Default is <c>src</c>.</para>
		/// </summary>
		public string PathAttribute
		{
			get { return _pathAttribute; }
			set { _pathAttribute = value; }
		}


		/// <summary>
		/// <para>Gets or sets whether the output HTML tag is self-closing or has a closing tag.</para>
		/// <para>This property is only considered if <see cref="TagName"/> is a non-zero-length <see cref="string"/>.</para>
		/// <para>Default is <c>false</c>.</para>
		/// </summary>
		public bool SelfCloseTag
		{
			get { return _selfCloseTag; }
			set { _selfCloseTag = value; }
		}


		/// <summary>
		/// <para>Gets or sets the name of the enclosing HTML tag.</para>
		/// <para>Default value is <c>script</c>.</para>
		/// </summary>
		public virtual string TagName
		{
			get { return _tagName; }
			set { _tagName = value; }
		}


		/// <summary>
		/// <para>Gets or sets a collection of key-value pairs which form the tag's attribute collection.</para>
		/// <para>Default value is <c>type="text/javascript>"</c>.</para>
		/// </summary>
		public virtual IEnumerable<KeyValuePair<string, string>> TagAttributes
		{
			get { return _tagAttributes; }
			set { _tagAttributes = value; }
		}

		#endregion


		/// <summary>
        /// <para>Executes this instance of <see cref="HtmlWriter"/> upon the specified argument object, storing the</para>
        /// <para>contents using the specified <see cref="IStore"/> implementation, and emitting an HTML tag which</para>
        /// <para>references the newly-stored external resource.</para>
        /// </summary>
        /// <param name="args">An instance of <see cref="PostModifierArgs"/>.</param>
        /// <returns>
        /// An HTML tag, whose name and attributes are specified in <see cref="PostModifierArgs.CustomObject"/>, and containing the contents as specified in <see cref="PostModifierArgs.Emitted"/>.
        /// </returns>
        public override IHtmlString Process(PostModifierArgs args)
        {
            // verify that we've been given an implementation of IStore to use

            _storeProvider = Configuration.Settings.Instance.StoreProvider;
            if (_storeProvider == null)
            {
                throw new NotSpecifiedException("The PostModifierArgs object must specify an implementation of SuperScript.ExternalFile.Storage.IStore.");
            }


            // get the full filename (including the file extension and query-string)

            var hash = string.Empty;
            if (String.IsNullOrWhiteSpace(FileName) || MakeUnique)
            {
                hash = CalculateMD5(args.Emitted);
            }
            var filename = FullFileName(hash);


            // write the contents and the content-type from PostModifierArgs using the storage provider

			WriteToStorage(new Storable
				               {
					               CacheForTimePeriod = CacheForTimePeriod,
					               ContentType = ContentType,
					               Contents = args.Emitted,
					               Key = filename,
					               Longevity = Longevity
				               });


            // write the HTML tag

            var shouldWriteTag = !String.IsNullOrWhiteSpace(TagName);
            if (!shouldWriteTag)
            {
                return new HtmlString(String.Empty);
            }

            var output = new StringBuilder();
            output.Append("<");                                         // open the tag
            output.Append(TagName);                           // tag name
            output.Append(" ");                                         // space between the tag name and the path attribute (i.e., 'src')
            output.Append(PathAttribute);                     // the name of the attribute which will contain the URL/path (typically 'src')
            output.Append("=\"");                                       // the '=' between the attribute's key and its value
            output.Append(HandlerPath);                       // the path prior to the file name
            output.Append(filename);                                    // the name of the external file  --  the HandlerPath the filename constitute the URL
            output.Append("\"");                                        // closing " after the path attribute's value
            AppendAttributes(ref output, TagAttributes);      // append any attributes to the URL

            // if this tag is self-closing...
            if (SelfCloseTag)
            {
                output.AppendLine(" />");

                return new HtmlString(output.ToString());
            }

            // otherwise add a closing tag
            output.Append("></");
            output.Append(TagName);
            output.AppendLine(">");

            return new HtmlString(output.ToString());
        }

		
		#region Private methods

		private void WriteToStorage(IStorable storable)
        {
            // store the contents
            _storeProvider.AddOrUpdate(storable);
        }


        /// <summary>
        /// Enumerates a collection of key-value pairs and appends them to the specified <see cref="StringBuilder"/> as HTML attributes.
        /// </summary>
        private void AppendAttributes(ref StringBuilder output, IEnumerable<KeyValuePair<string, string>> tagAttributes)
        {
            var attrs = tagAttributes as KeyValuePair<string, string>[] ?? tagAttributes.ToArray();
            if (!attrs.Any())
            {
                return;
            }

            foreach (var tagAttribute in attrs.Where(ta => !string.IsNullOrWhiteSpace(ta.Key)))
            {
                output.Append(" ");
                output.Append(tagAttribute.Key);

                if (string.IsNullOrWhiteSpace(tagAttribute.Value))
                {
                    continue;
                }

                output.Append("=\"");
                output.Append(tagAttribute.Value);
                output.Append("\"");
            }
        }

		
		// We're using MD5 rather than the more secure SHA because security isn't important; we simply want to produce
		// a hash of the file contents as quickly and efficiently as possible.
        private string CalculateMD5(string dataToCalculate)
		{
			using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(dataToCalculate));

				// we need to remove any non-alphanumeric characters from the sequence, otherwise it may not be a valid HTTP request
	            return Regex.Replace(Convert.ToBase64String(hash),
	                                 "[^a-zA-Z0-9_.]+",
	                                 "",
	                                 RegexOptions.Compiled);
            }
        }


	    /// <summary>
	    /// Uses the other properties (<see cref="FileName"/>, <see cref="FileExtension"/>, <see cref="MakeUnique"/> and <see cref="Longevity"/>) to build the full filename.
	    /// </summary>
	    /// <param name="hash">Expected to be a hash of the contents stored within the file, though this can be anything unique.</param>
	    /// <returns>A unique filename.</returns>
	    /// <exception cref="ArgumentException">Thrown if the specified <see cref="hash"/> parameter is empty while the <see cref="FileName"/> property is also empty.</exception>
	    private string FullFileName(string hash)
	    {
		    string ffn;
		    if (String.IsNullOrEmpty(FileName))
		    {
			    if (String.IsNullOrWhiteSpace(hash))
			    {
				    throw new ArgumentException("The specified hash cannot be empty if the FileName property is also empty.");
			    }

			    ffn = hash;
		    }
		    else
		    {
			    ffn = FileName;
		    }

		    return ffn
		           + (!String.IsNullOrEmpty(FileExtension)
			              ? "." + FileExtension
			              : String.Empty)
		           + GetQueryString(hash);
	    }


	    private string GetQueryString(string hash)
		{
			var qs = new QueryString();

			// if we're not using the checksum/hash for the filename, and a checksum/hash has been requested
			// then append it as a query-string
			if (!String.IsNullOrWhiteSpace(FileName) && MakeUnique && !String.IsNullOrWhiteSpace(hash))
			{
				qs.Add(QueryStringKeys.UniqueKey, hash);
			}

			return qs.ToString();
		}

		#endregion
    }
}