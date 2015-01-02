using System;

namespace SuperScript.ExternalFile.Storage
{
    public interface IStorable
	{
		/// <summary>
		/// The period for which the client should cache the file.
		/// </summary>
		TimeSpan CacheForTimePeriod { get; set; }


		/// <summary>
		/// This property is required for correct deserialisation.
		/// </summary>
		string CacheForTimePeriod_Serialize { get; set; }


        /// <summary>
        /// Gets or sets the contents of the file to be stored.
        /// </summary>
        string Contents { get; set; }


        /// <summary>
        /// Gets or sets the MIME type that the content will be sent with.
        /// </summary>
		string ContentType { get; set; }


		/// <summary>
		/// <para>Gets or sets the key for this instance of <see cref="IStorable"/>.</para>
		/// <para>The <see cref="Key"/> is a unique string by which this <see cref="IStorable"/> is addressed.</para>
		/// </summary>
		string Key { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="Longevity"/> which determines the lifespan of the stored contents.
        /// </summary>
        Longevity Longevity { get; set; }
    }
}