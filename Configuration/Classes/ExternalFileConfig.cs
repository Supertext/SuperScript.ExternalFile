using System.Configuration;

namespace SuperScript.ExternalFile.Configuration
{
    /// <summary>
	/// This class represents the superScript.ExternalFile configuration section from the web.config file.
    /// </summary>
    public class ExternalFileConfig : ConfigurationSection
    {
        private static readonly ConfigurationProperty StorageElement = new ConfigurationProperty("storage", typeof (StorageElement), null, ConfigurationPropertyOptions.None);
        

        [ConfigurationProperty("storage", IsRequired = false)]
        public StorageElement StorageProvider
        {
            get { return (StorageElement) this[StorageElement]; }
        }
    }
}