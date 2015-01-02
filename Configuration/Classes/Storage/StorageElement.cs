using System;
using System.ComponentModel;
using System.Configuration;
using SuperScript.Configuration;

namespace SuperScript.ExternalFile.Configuration
{
    public class StorageElement : ConfigurationElement, IAssemblyElement
    {
		private static readonly ConfigurationProperty DbProviderElement = new ConfigurationProperty("dbProvider", typeof(DbProviderElement), null, ConfigurationPropertyOptions.None);
		private static readonly ConfigurationProperty PropertiesElement = new ConfigurationProperty("properties", typeof(PropertyCollection), null, ConfigurationPropertyOptions.None);


	    [ConfigurationProperty("properties", IsRequired = false)]
	    public PropertyCollection CustomProperties
	    {
		    get { return (PropertyCollection) this[PropertiesElement]; }
	    }


	    [ConfigurationProperty("emptyOnStartup", IsRequired = false, DefaultValue = true)]
	    public bool EmptyOnStartup
	    {
		    get { return (bool) this["emptyOnStartup"]; }
	    }


	    [ConfigurationProperty("dbProvider", IsRequired = false)]
	    public DbProviderElement DbProvider
	    {
		    get { return (DbProviderElement) this[DbProviderElement]; }
	    }


	    [ConfigurationProperty("type", IsRequired = false, DefaultValue = "SuperScript.ExternalFile.Storage.IsoStore, SuperScript.ExternalFile")]
        [TypeConverter(typeof (TypeNameConverter))]
        public Type Type
        {
            get { return (Type) this["type"]; }
        }
    }
}