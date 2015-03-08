using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using SuperScript.ExternalFile.Storage.Exceptions;


namespace SuperScript.ExternalFile.Storage
{
    /// <summary>
    /// Uses file storage as the implementation of <see cref="IStore"/>.
    /// </summary>
    public class FileStore : IStore
    {
        #region Global variables

        private string _directoryName = "superScript_ExternalFiles";
	    private bool _init;

        #endregion


        #region Properties

	    /// <summary>
	    /// <para>Gets or sets the top-level directory name in the file storage.</para>
	    /// <para>Default is "superScript_ExternalFiles".</para>
	    /// </summary>
	    public string StoreName
	    {
		    get { return _directoryName; }
		    set { _directoryName = value; }
	    }


	    private string DirectoryPath
	    {
		    get
		    {
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, StoreName);
		    }
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
			    throw new MissingStorageConfigurationException("No StoreName has been set on the instance of FileStore.");
		    }

			var path = Path.Combine(DirectoryPath, storable.Key.Replace("?", string.Empty));
		    using (var fs = File.Create(path))
		    {
			    var json = new JavaScriptSerializer().Serialize(storable);
			    var bytesJson = new UTF8Encoding(true).GetBytes(json);
			    fs.Write(bytesJson, 0, bytesJson.Length);
		    }
	    }


	    private void CreateDirectory()
	    {
			Directory.CreateDirectory(DirectoryPath);

			if (!Directory.Exists(DirectoryPath))
			{
				throw new StorageDirectoryCreationException();
			}
	    }


	    /// <summary>
        /// Deletes the specified instance of <see cref="IStorable"/>.
        /// </summary>
		/// <param name="key">The unique identifier that the <see cref="IStorable"/> was stored under.</param>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
        /// <exception cref="MissingStorageConfigurationException">Thrown when the <see cref="StoreName"/> property has not been set.</exception>
        public void Delete(string key)
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

            if (String.IsNullOrWhiteSpace(StoreName))
            {
                throw new MissingStorageConfigurationException("No StoreName has been set on the instance of FileStore.");
            }

			File.Delete(Path.Combine(DirectoryPath, key.Replace("?", string.Empty)));
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
			CreateDirectory();
        }


	    /// <summary>
	    /// <para>Gets the instance of <see cref="IStorable"/> with the specified key.</para>
	    /// <para>Returns null if no matching keys were found.</para>
	    /// </summary>
		/// <param name="key">The unique identifier that the <see cref="IStorable"/> was stored under.</param>
		/// <exception cref="NotInitialisedException">Thrown when this instance of <see cref="IStore"/> has not been initialised.</exception>
	    /// <exception cref="MissingStorageConfigurationException">Thrown when the <see cref="StoreName"/> property has not been set.</exception>
	    public IStorable Get(string key)
		{
			if (!_init)
			{
				throw new NotInitialisedException();
			}

		    if (String.IsNullOrWhiteSpace(StoreName))
		    {
			    throw new MissingStorageConfigurationException("No StoreName has been set on the instance of FileStore.");
		    }

			var path = Path.Combine(DirectoryPath, key.Replace("?", string.Empty));

		    if (!File.Exists(path))
		    {
			    return null;
		    }

		    var json = File.ReadAllText(path);

		    return new JavaScriptSerializer().Deserialize<Storable>(json);
	    }



	    /// <summary>
	    /// Returns a snapshot of all <see cref="IStorable"/> instances in the store.
	    /// </summary>
	    public IEnumerable<IStorable> GetAll()
	    {
		    if (!_init)
		    {
			    throw new NotInitialisedException();
		    }

		    if (String.IsNullOrWhiteSpace(StoreName))
		    {
			    throw new MissingStorageConfigurationException("No StoreName has been set on the instance of FileStore.");
		    }

			// there should be no sub-directories, only files

		    var filePaths = Directory.EnumerateFiles(DirectoryPath).ToArray();
		    var files = new List<IStorable>(filePaths.Length);
		    foreach (var file in filePaths)
		    {
			    var json = File.ReadAllText(file);

			    files.Add(new JavaScriptSerializer().Deserialize<Storable>(json));
		    }

		    return files;
	    }


	    /// <summary>
	    /// Performs any initilisation that the store requires before items may be stored and retrieved.
	    /// </summary>
	    public void Init()
	    {
		    if (String.IsNullOrWhiteSpace(StoreName))
		    {
			    throw new ConfigurablePropertyNotSpecifiedException("The StoreName property must be specified.");
		    }

		    CreateDirectory();

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

			if (String.IsNullOrWhiteSpace(StoreName))
			{
				throw new MissingStorageConfigurationException("No StoreName has been set on the instance of FileStore.");
			}

			new DirectoryInfo(DirectoryPath).Delete(true);
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
                throw new MissingStorageConfigurationException("No StoreName has been set on the instance of FileStore.");
            }

            // there should be no sub-directories, only files

            var olderThan = DateTime.Now.Subtract(removeThreshold);
            var info = new DirectoryInfo(DirectoryPath);
            var fileInfos = info.GetFiles().Where(p => p.CreationTime <= olderThan).ToArray();
            foreach (var file in fileInfos)
            {
                File.Delete(file.FullName);
            }
        }

        #endregion
    }
}