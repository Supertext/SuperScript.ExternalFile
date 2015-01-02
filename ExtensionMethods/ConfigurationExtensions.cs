using System;
using SuperScript.Configuration;
using SuperScript.ExtensionMethods;
using SuperScript.ExternalFile.Configuration;
using SuperScript.ExternalFile.Storage;

namespace SuperScript.ExternalFile.ExtensionMethods
{
    /// <summary>
    /// Contains extension methods which can be invoked upon the classes which are implemented in the web.config &lt;superScript&gt; section.
    /// </summary>
    public static class ConfigurationExtensions
	{
	    /// <summary>
	    /// Creates an instance of <see cref="IDbStoreProvider"/> from the specified <see cref="DbProviderElement"/> object.
	    /// </summary>
	    public static IDbStoreProvider ToDbProvider(this DbProviderElement dbProviderElement)
	    {
		    if (dbProviderElement == null)
		    {
			    return null;
		    }

		    // create the instance...
		    var instance = dbProviderElement.ToInstance<IDbStoreProvider>();

		    // check for the connection string
		    if (dbProviderElement.ConnectionString != null)
		    {
			    // first priority is the ConnectionString property which might contain the full connection string
			    if (!String.IsNullOrWhiteSpace(dbProviderElement.ConnectionString))
			    {
				    instance.ConnectionString = dbProviderElement.ConnectionString;
			    }
				    // otherwise check if the name of a configured connection string has been supplied
			    else if (!String.IsNullOrWhiteSpace(dbProviderElement.ConnectionStringName))
			    {
				    var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[dbProviderElement.ConnectionStringName].ToString();
				    if (!String.IsNullOrWhiteSpace(connectionString))
				    {
					    instance.ConnectionString = connectionString;
				    }
				}
				else
				{
					throw new ConfigurablePropertyNotSpecifiedException("The <dbProvider> element must have either the connectionString or connectionStringName attribute populated.");
				}
		    }

		    // add the database name if it has been specified
		    // (only needs specifying if differs from, or is not supplied in, the connection string)
		    if (dbProviderElement.DbName != null)
		    {
			    if (!String.IsNullOrWhiteSpace(dbProviderElement.DbName))
			    {
				    instance.DbName = dbProviderElement.DbName;
			    }
		    }

		    return instance;
	    }


	    /// <summary>
	    /// Creates an instance of <see cref="IStore"/> from the specified <see cref="StorageElement"/> object.
	    /// </summary>
	    public static IStore ToStorageProvider(this StorageElement storageElement)
	    {
		    // create the instance...
		    var instance = storageElement.ToInstance<IStore>();

		    if (instance is IDbStore && storageElement.DbProvider != null)
		    {
			    ((IDbStore) instance).DbStoreProvider = storageElement.DbProvider.ToDbProvider();
		    }

		    // populate any properties which were declared for the context StorageElement
		    storageElement.CustomProperties.AssignProperties(instance);

		    return instance;
	    }


        /// <summary>
        /// Returns the <see cref="Type"/> specified in the <see cref="IAssemblyElement"/>.
        /// </summary>
        public static T ToInstance<T>(this IAssemblyElement element)
        {
            return (T) Activator.CreateInstance(element.Type);
        }
    }
}