using System;
using System.Web.Script.Serialization;

namespace SuperScript.ExternalFile.Storage
{
    public class Storable : IStorable
    {
	    /// <summary>
	    /// The period for which the client should cache the file.
	    /// </summary>
	    [ScriptIgnore]
	    public TimeSpan CacheForTimePeriod { get; set; }


		/// <summary>
		/// This property is required for correct deserialisation.
		/// </summary>
	    public string CacheForTimePeriod_Serialize
	    {
		    get
		    {
			    // d:hh:mm:ss
			    return String.Format("{0}:{1}:{2}:{3}",
			                         CacheForTimePeriod.Days,
			                         CacheForTimePeriod.Hours,
			                         CacheForTimePeriod.Minutes,
			                         CacheForTimePeriod.Seconds);
		    }
		    set
		    {
			    CacheForTimePeriod = TimeSpan.Parse(value, null);
		    }
	    }


	    /// <summary>
        /// Gets or sets the contents of the file to be stored.
        /// </summary>
        public string Contents { get; set; }


        /// <summary>
        /// Gets or sets the MIME type that the content will be sent with.
        /// </summary>
        public string ContentType { get; set; }


		/// <summary>
		/// <para>Gets or sets the key for this instance of <see cref="IStorable"/>.</para>
		/// <para>The <see cref="Key"/> is a unique string by which this <see cref="IStorable"/> is addressed.</para>
		/// </summary>
		public string Key { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="Longevity"/> which determines the lifespan of the stored contents.
        /// </summary>
        public Longevity Longevity { get; set; }
    }
}