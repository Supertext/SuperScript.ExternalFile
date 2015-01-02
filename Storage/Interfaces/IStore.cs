using System.Collections.Generic;

namespace SuperScript.ExternalFile.Storage
{
    public interface IStore
    {
		/// <summary>
		/// Adds the specified instance of <see cref="IStorable"/> to the store using the specified key. If an 
		/// item exists with the specified <see cref="IStorable.Key"/> then it will be updated.
        /// </summary>
        /// <param name="storable">An instance of <see cref="IStorable"/> which contains all pertinent data.</param>
        void AddOrUpdate(IStorable storable);


        /// <summary>
        /// Deletes the specified instance of <see cref="IStorable"/>.
        /// </summary>
        /// <param name="key">The unique identifier that the <see cref="IStorable"/> was stored under.</param>
        void Delete(string key);


        /// <summary>
        /// Gets or sets the name of the top-level directory inside the storage provider, should it be required.
        /// </summary>
        string StoreName { get; set; }


        /// <summary>
        /// Removes all files and directories from the store.
        /// </summary>
        void Empty();


        /// <summary>
        /// <para>Gets the instance of <see cref="IStorable"/> with the specified key.</para>
		/// <para>Returns <c>null</c> if no matching keys were found.</para>
        /// </summary>
        /// <param name="key">The unique identifier that the <see cref="IStorable"/> was stored under.</param>
		IStorable Get(string key);


		/// <summary>
		/// Returns a snapshot of all <see cref="IStorable"/> instances in the store.
		/// </summary>
	    IEnumerable<IStorable> GetAll();


		/// <summary>
		/// Performs any initilisation that the store requires before items may be stored and retrieved.
		/// </summary>
		void Init();


		/// <summary>
		/// Removes all traces of the store.
		/// </summary>
	    void Remove();
    }
}