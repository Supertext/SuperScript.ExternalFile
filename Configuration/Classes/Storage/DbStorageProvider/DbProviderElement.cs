using System;
using System.ComponentModel;
using System.Configuration;
using SuperScript.Configuration;

namespace SuperScript.ExternalFile.Configuration
{
	public class DbProviderElement : ConfigurationElement, IAssemblyElement
	{
		[ConfigurationProperty("connectionString", IsRequired = false)]
		public string ConnectionString
		{
			get { return (string) this["connectionString"]; }
		}


		[ConfigurationProperty("connectionStringName", IsRequired = false)]
		public string ConnectionStringName
		{
			get { return (string) this["connectionStringName"]; }
		}


		[ConfigurationProperty("dbName", IsRequired = false)]
		public string DbName
		{
			get { return (string) this["dbName"]; }
		}


		[ConfigurationProperty("type", IsRequired = true)]
		[TypeConverter(typeof (TypeNameConverter))]
		public Type Type
		{
			get { return (Type) this["type"]; }
		}
	}
}