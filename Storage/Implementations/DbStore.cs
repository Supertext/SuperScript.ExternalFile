using System;
using System.Collections.Generic;

namespace SuperScript.ExternalFile.Storage
{
	public class DbStore : IDbStore
	{
		#region Global variables

		private bool _init;
		private string _storeName = "superscript_externalfiles";

		#endregion


		#region Properties


		/// <summary>
		/// An instance of <see cref="IDbStoreProvider"/> which contains the instructions for initialising a database-based implementation of <see cref="IStore"/>.
		/// </summary>
		public IDbStoreProvider DbStoreProvider { get; set; }


		/// <summary>
		/// <para>Gets or sets the name of the table inside the database.</para>
		/// <para>Default value is "superscript_externalfiles".</para>
		/// </summary>
		public string StoreName
		{
			get { return _storeName; }
			set { _storeName = value.ToLower(); }
		}

		#endregion


		#region Methods

		/// <summary>
		/// Adds the specified instance of <see cref="IStorable"/> to the store using the specified key. If an 
		/// item exists with the specified <see cref="IStorable.Key"/> then it will be updated.
		/// </summary>
		/// <param name="storable">An instance of <see cref="IStorable"/> which contains all pertinent data.</param>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="ConfigurablePropertyNotSpecifiedException">Thrown if <see cref="DbStoreProvider"/> is null.</exception>
		public void AddOrUpdate(IStorable storable)
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (DbStoreProvider == null)
			{
				throw new ConfigurablePropertyNotSpecifiedException("No instance of DbStoreProvider has been configured on the DbStore object.");
			}

			DbStoreProvider.AddOrUpdate(storable);
		}


		/// <summary>
		/// Deletes the instance of <see cref="IStorable"/> which has been stored against the specified <see cref="key"/>.
		/// </summary>
		/// <param name="key">The unique identifier that the <see cref="IStorable"/> was stored under.</param>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="ConfigurablePropertyNotSpecifiedException">Thrown if <see cref="DbStoreProvider"/> is null.</exception>
		public void Delete(string key)
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (DbStoreProvider == null)
			{
				throw new ConfigurablePropertyNotSpecifiedException("No instance of DbStoreProvider has been configured on the DbStore object.");
			}

			DbStoreProvider.Delete(key);
		}


		/// <summary>
		/// Removes all files and directories from the store.
		/// </summary>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="ConfigurablePropertyNotSpecifiedException">Thrown if <see cref="DbStoreProvider"/> is null.</exception>
		public void Empty()
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			Remove();
			DbStoreProvider.Init();
		}


		/// <summary>
		/// <para>Gets the instance of <see cref="IStorable"/> with the specified key.</para>
		/// <para>Returns null if no matching keys were found.</para>
		/// </summary>
		/// <param name="key">The unique identifier that the <see cref="IStorable"/> was stored under.</param>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="ConfigurablePropertyNotSpecifiedException">Thrown if <see cref="DbStoreProvider"/> is null.</exception>
		public IStorable Get(string key)
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (DbStoreProvider == null)
			{
				throw new ConfigurablePropertyNotSpecifiedException("No instance of DbStoreProvider has been configured on the DbStore object.");
			}

			return DbStoreProvider.Get(key);
		}


		/// <summary>
		/// Returns a snapshot of all <see cref="IStorable"/> instances in the store.
		/// </summary>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="ConfigurablePropertyNotSpecifiedException">Thrown if <see cref="DbStoreProvider"/> is null.</exception>
		public IEnumerable<IStorable> GetAll()
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (DbStoreProvider == null)
			{
				throw new ConfigurablePropertyNotSpecifiedException("No instance of DbStoreProvider has been configured on the DbStore object.");
			}

			return DbStoreProvider.GetAll();
		}


		/// <summary>
		/// Performs any initilisation that the store requires before items may be stored and retrieved.
		/// </summary>
		/// <exception cref="ConfigurablePropertyNotSpecifiedException">Thrown when <see cref="DbStoreProvider"/> has not been set.</exception>
		/// <exception cref="ConfigurablePropertyNotSpecifiedException">Thrown when the <see cref="StoreName"/> has not been set.</exception>
		public void Init()
		{
			if (DbStoreProvider == null)
			{
				throw new ConfigurablePropertyNotSpecifiedException("No instance of DbStoreProvider has been configured on the DbStore object.");
			}

			if (String.IsNullOrWhiteSpace(StoreName))
			{
				throw new ConfigurablePropertyNotSpecifiedException("The StoreName property must be specified.");
			}
			DbStoreProvider.StoreName = StoreName;

			DbStoreProvider.Init();

			_init = true;
		}


		/// <summary>
		/// Removes all traces of the store.
		/// </summary>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="ConfigurablePropertyNotSpecifiedException">Thrown if <see cref="DbStoreProvider"/> is null.</exception>
		public void Remove()
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (DbStoreProvider == null)
			{
				throw new ConfigurablePropertyNotSpecifiedException("No instance of DbStoreProvider has been configured on the DbStore object.");
			}

			DbStoreProvider.DeleteStore();
		}


	    /// <summary>
	    /// Removes instances of <see cref="IStorable"/> which are older than the specified <see cref="TimeSpan"/>.
	    /// </summary>
	    /// <param name="removeThreshold">Instances of <see cref="IStorable"/> which are older than this will be removed from the store.</param>
	    public void Scavenge(TimeSpan removeThreshold)
        {
            if (!_init)
            {
                throw new NotInitialisedException();
            }

            if (DbStoreProvider == null)
            {
                throw new ConfigurablePropertyNotSpecifiedException("No instance of DbStoreProvider has been configured on the DbStore object.");
            }

            DbStoreProvider.Scavenge(removeThreshold);
	    }

	    #endregion
	}
}