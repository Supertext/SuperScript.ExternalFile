using System.Collections.Generic;

namespace SuperScript.ExternalFile.Storage
{
	/// <summary>
	/// Contains the instructions for initialising a database-based implementation of <see cref="IStore"/>.
	/// </summary>
	/// <returns><c>True</c> if the store was created, <c>false</c> otherwise.</returns>
	public interface IDbStoreProvider
	{
		/// <summary>
		/// Adds the specified instance of <see cref="IStorable"/> to the store using the specified key. If an 
		/// item exists with the specified <see cref="IStorable.Key"/> then it will be updated.
		/// </summary>
		/// <param name="storable">An instance of <see cref="IStorable"/> which contains all pertinent data.</param>
		void AddOrUpdate(IStorable storable);


		/// <summary>
		/// Gets or sets the connection string that will be used to communicate with the underlying database.
		/// </summary>
		string ConnectionString { get; set; }


		/// <summary>
		/// Gets or sets the name of the database, if this is not already detailed in the connection string.
		/// </summary>
		string DbName { get; set; }


		/// <summary>
		/// Deletes the instance of <see cref="IStorable"/> which has been stored against the specified <see cref="key"/>.
		/// </summary>
		/// <param name="key">The unique identifier that the <see cref="IStorable"/> was stored under.</param>
		void Delete(string key);


		/// <summary>
		/// Deletes the entire store from the database.
		/// </summary>
		void DeleteStore();


		/// <summary>
		/// Deletes the entire store from the database.
		/// </summary>
		IStorable Get(string key);


		/// <summary>
		/// Returns a snapshot of all <see cref="IStorable"/> instances in the store.
		/// </summary>
		IEnumerable<IStorable> GetAll();


		/// <summary>
		/// Checks that the store (a database table) exists. If not, the store will be created.
		/// </summary>
		void Init();


		/// <summary>
		/// <para>Gets or sets the name of the table inside the database.</para>
		/// <para>Default value is "superScript_ExternalFiles".</para>
		/// </summary>
		string StoreName { get; set; }
	}
}