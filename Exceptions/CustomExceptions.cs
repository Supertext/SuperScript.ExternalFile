using System;
using SuperScript.Emitters;
using SuperScript.ExternalFile.Storage;

namespace SuperScript.ExternalFile
{
    /// <summary>
    /// An <see cref="Exception"/> indicating that no instances of <see cref="IEmitter"/> have been configured.
    /// </summary>
    public class MissingConfigurationObjectException : SuperScriptException
    {
        /// <summary>
        /// Constructor for <see cref="MissingConfigurationObjectException"/> which allows an exception-specific message to be relayed to the developer.
        /// </summary>
        public MissingConfigurationObjectException(string message)
            : base(message)
        { }


        public MissingConfigurationObjectException()
            : base("No instance of ExternalFileProperties was passed.")
        {}
    }


	/// <summary>
	/// An <see cref="Exception"/> indicating that a required database has not been configured correctly.
	/// </summary>
	public class MissingDatabaseConfigurationException : SuperScriptException
	{
		/// <summary>
		/// Constructor for <see cref="MissingDatabaseConfigurationException"/> which allows an exception-specific message to be relayed to the developer.
		/// </summary>
		public MissingDatabaseConfigurationException(string message)
			: base(message)
		{ }


		public MissingDatabaseConfigurationException()
			: base("Bad database connection information was specified.")
		{ }
	}


	/// <summary>
	/// An <see cref="Exception"/> indicating that the implementation of <see cref="IStore"/> has not been initialised.
	/// </summary>
	public class NotInitialisedException : SuperScriptException
	{
		/// <summary>
		/// Constructor for <see cref="NotInitialisedException"/> which allows an exception-specific message to be relayed to the developer.
		/// </summary>
		public NotInitialisedException(string message)
			: base(message)
		{ }


		public NotInitialisedException()
			: base("The current instance of IStore has not been initialised. This can be achieved by called IStore.Init().")
		{ }
	}


	/// <summary>
	/// An <see cref="Exception"/> indicating that the store could not be created.
	/// </summary>
	public class UnableToCreateStoreException : SuperScriptException
	{
		/// <summary>
		/// Constructor for <see cref="UnableToCreateStoreException"/> which allows an exception-specific message to be relayed to the developer.
		/// </summary>
		public UnableToCreateStoreException(string message)
			: base(message)
		{ }


		public UnableToCreateStoreException()
			: base("The store could not be created.")
		{ }
	}
}