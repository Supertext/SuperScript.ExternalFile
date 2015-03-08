using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Web.Script.Serialization;
using SuperScript.ExternalFile.Storage.Exceptions;

namespace SuperScript.ExternalFile.Storage
{
	/// <summary>
	/// Uses Isolated Storage as the implementation of <see cref="IStore"/>.
	/// </summary>
	public class IsoStore : IStore
	{
		#region Global variables

		private string _directoryName = "superScript";
		private bool _init;
		private bool _storeExists;

		#endregion


		#region Properties

		/// <summary>
		/// Gets or sets the top-level directory name in the Isolated Storage.
		/// </summary>
		public string StoreName
		{
			get { return _directoryName; }
			set { _directoryName = value; }
		}

		#endregion


		#region Methods

		/// <summary>
		/// Adds the specified instance of <see cref="IStorable"/> to the store using the specified key. If an 
		/// item exists with the specified <see cref="key"/> then it will be updated.
		/// </summary>
		/// <param name="storable">An instance of <see cref="IStorable"/> which contains all pertinent data.</param>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="MissingStorageConfigurationException">Thrown when the <see cref="StoreName"/> property has not been set.</exception>
		/// <exception cref="MissingStorageDirectoryException">Thrown when the top-level directory has not been created.</exception>
		public void AddOrUpdate(IStorable storable)
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (String.IsNullOrWhiteSpace(StoreName))
			{
				throw new MissingStorageConfigurationException("No StoreName has been set on the instance of IsoStore.");
			}

			if (!_storeExists)
			{
				throw new MissingStorageDirectoryException();
			}

			var json = new JavaScriptSerializer().Serialize(storable);

			using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
			{
				// if there is a '?' in the fileName (for a query-string) then remove it

				var isoFileStream = isoStore.CreateFile(StoreName + "//" + storable.Key.Replace("?", string.Empty));

				var writer = new StreamWriter(isoFileStream);
				writer.Write(json);
				writer.Close();
			}
		}


		private void CreateStore()
		{
			if (String.IsNullOrWhiteSpace(StoreName))
			{
				throw new ConfigurablePropertyNotSpecifiedException("The StoreName property must be specified.");
			}

			using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
			{
				if (!isoStore.DirectoryExists(StoreName))
				{
					isoStore.CreateDirectory(StoreName);
				}

				_storeExists = isoStore.DirectoryExists(StoreName);
			}
		}


		/// <summary>
		/// Deletes the specified instance of <see cref="IStorable"/>.
		/// </summary>
		/// <param name="key">The unique identifier that the <see cref="IStorable"/> was stored under.</param>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="MissingStorageConfigurationException">Thrown when the <see cref="StoreName"/> property has not been set.</exception>
		/// <exception cref="MissingStorageDirectoryException">Thrown when the top-level directory has not been created.</exception>
		public void Delete(string key)
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (String.IsNullOrWhiteSpace(StoreName))
			{
				throw new MissingStorageConfigurationException("No StoreName has been set on the instance of IsoStore.");
			}

			if (!_storeExists)
			{
				throw new MissingStorageDirectoryException();
			}

			using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
			{
				var fullName = StoreName + "//" + key.Replace("?", string.Empty);

				if (isoStore.FileExists(fullName))
				{
					isoStore.DeleteFile(fullName);
				}
			}
		}


		/// <summary>
		/// Removes all files and directories from the store.
		/// </summary>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="MissingStorageConfigurationException">Thrown when the <see cref="StoreName"/> property has not been set.</exception>
		public void Empty()
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			Remove();
			CreateStore();
		}


		/// <summary>
		/// Recursively iterates through each directory and sub-directory, deleting all files and then each of the directories.
		/// </summary>
		private void EmptyRecursive(string directoryName)
		{
			using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
			{
				var files = isoStore.GetFileNames(directoryName + @"/*");
				foreach (var file in files)
				{
					isoStore.DeleteFile(directoryName + @"/" + file);
				}

				var subDirectories = isoStore.GetDirectoryNames(directoryName + @"/*");
				foreach (var subDirectory in subDirectories)
				{
					EmptyRecursive(directoryName + @"/" + subDirectory);
				}

				isoStore.DeleteDirectory(directoryName);
			}
		}


		/// <summary>
		/// <para>Gets the instance of <see cref="IStorable"/> with the specified key.</para>
		/// <para>Returns null if no matching keys were found.</para>
		/// </summary>
		/// <param name="key">The unique identifier that the <see cref="IStorable"/> was stored under.</param>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="MissingStorageConfigurationException">Thrown when the <see cref="StoreName"/> property has not been set.</exception>
		/// <exception cref="MissingStorageDirectoryException">Thrown when the top-level directory has not been created.</exception>
		public IStorable Get(string key)
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (String.IsNullOrWhiteSpace(StoreName))
			{
				throw new MissingStorageConfigurationException("No StoreName has been set on the instance of IsoStore.");
			}

			if (!_storeExists)
			{
				throw new MissingStorageDirectoryException();
			}

			using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
			{
				var fullName = StoreName + "//" + key.Replace("?", string.Empty);

				if (!isoStore.FileExists(fullName))
				{
					return null;
				}

				using (var isoStream = new IsolatedStorageFileStream(fullName, FileMode.Open, isoStore))
				{
					using (var reader = new StreamReader(isoStream))
					{
						return new JavaScriptSerializer().Deserialize<Storable>(reader.ReadToEnd());
					}
				}
			}
		}


		/// <summary>
		/// Returns a snapshot of all <see cref="IStorable"/> instances in the store.
		/// </summary>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="MissingStorageConfigurationException">Thrown when the <see cref="StoreName"/> property has not been set.</exception>
		/// <exception cref="MissingStorageDirectoryException">Thrown when the top-level directory has not been created.</exception>
		public IEnumerable<IStorable> GetAll()
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (String.IsNullOrWhiteSpace(StoreName))
			{
				throw new MissingStorageConfigurationException("No StoreName has been set on the instance of IsoStore.");
			}

			if (!_storeExists)
			{
				throw new MissingStorageDirectoryException();
			}

			using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
			{
				var fileNames = isoStore.GetFileNames(StoreName + @"/*");
				var files = new List<IStorable>(fileNames.Length);
				foreach (var filename in fileNames)
				{
					var fullName = StoreName + "//" + filename;

					using (var isoStream = new IsolatedStorageFileStream(fullName, FileMode.Open, isoStore))
					{
						using (var reader = new StreamReader(isoStream))
						{
							files.Add(new JavaScriptSerializer().Deserialize<Storable>(reader.ReadToEnd()));
						}
					}
				}

				return files;
			}
		}


		/// <summary>
		/// Performs any initilisation that the store requires before items may be stored and retrieved.
		/// </summary>
		/// <exception cref="ConfigurablePropertyNotSpecifiedException">Thrown when the <see cref="StoreName"/> property has not been set.</exception>
		public void Init()
		{
			CreateStore();

			_init = true;
		}


		/// <summary>
		/// Removes all traces of the store.
		/// </summary>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
		/// <exception cref="MissingStorageConfigurationException">Thrown when the <see cref="StoreName"/> property has not been set.</exception>
		public void Remove()
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

			if (!_storeExists)
			{
				return;
			}

			if (String.IsNullOrWhiteSpace(StoreName))
			{
				throw new MissingStorageConfigurationException("No StoreName has been set on the instance of IsoStore.");
			}

			EmptyRecursive(StoreName);
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

            if (String.IsNullOrWhiteSpace(StoreName))
            {
                throw new MissingStorageConfigurationException("No StoreName has been set on the instance of IsoStore.");
            }

            if (!_storeExists)
            {
                throw new MissingStorageDirectoryException();
            }

            using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
            {
                var olderThan = DateTime.Now.Subtract(removeThreshold);
                var fileNames = isoStore.GetFileNames(StoreName + @"/*");
                foreach (var fullName in fileNames.Select(filename => StoreName + "//" + filename)
                                                  .Where(fullName => isoStore.GetCreationTime(fullName) <= olderThan))
                {
                    isoStore.DeleteFile(fullName);
                }
            }
        }

		#endregion
	}
}