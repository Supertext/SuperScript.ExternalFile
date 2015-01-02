using System;

namespace SuperScript.ExternalFile.Storage.Exceptions
{
    /// <summary>
    /// An <see cref="Exception"/> indicating that the implementation of <see cref="IStore"/> has not been fully configured.
    /// </summary>
    public class MissingStorageConfigurationException : SuperScriptException
    {
        /// <summary>
        /// Constructor for <see cref="MissingStorageConfigurationException"/> which allows an exception-specific message to be relayed to the developer.
        /// </summary>
        public MissingStorageConfigurationException(string message)
            : base(message)
        { }


        public MissingStorageConfigurationException()
            : base("The implementation of IStore has not been fully configured.")
        {}
    }


    /// <summary>
    /// An <see cref="Exception"/> indicating that the top-level directory required for the storage does not exist.
    /// </summary>
    public class MissingStorageDirectoryException : SuperScriptException
    {
        /// <summary>
        /// Constructor for <see cref="MissingStorageDirectoryException"/> which allows an exception-specific message to be relayed to the developer.
        /// </summary>
        public MissingStorageDirectoryException(string message)
            : base(message)
        { }


        public MissingStorageDirectoryException()
            : base("The top-level directory for the storage has not been created.")
        { }
    }


	/// <summary>
	/// An <see cref="Exception"/> indicating that the top-level directory cannot be created.
	/// </summary>
	public class StorageDirectoryCreationException : SuperScriptException
	{
		/// <summary>
		/// Constructor for <see cref="MissingStorageDirectoryException"/> which allows an exception-specific message to be relayed to the developer.
		/// </summary>
		public StorageDirectoryCreationException(string message)
			: base(message)
		{ }


		public StorageDirectoryCreationException()
			: base("The top-level directory for the storage cannot be created.")
		{ }
	}
}