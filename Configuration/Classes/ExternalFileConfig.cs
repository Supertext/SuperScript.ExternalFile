using System.Configuration;


namespace SuperScript.ExternalFile.Configuration
{
    /// <summary>
    /// This class represents the superScript.ExternalFile configuration section from the web.config file.
    /// </summary>
    public class ExternalFileConfig : ConfigurationSection
    {
        private static readonly ConfigurationProperty StorageElement = new ConfigurationProperty("storage", typeof (StorageElement), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty ScavengerElement = new ConfigurationProperty("scavenger", typeof (ScavengerElement), null, ConfigurationPropertyOptions.None);


        [ConfigurationProperty("scavenger", IsRequired = false)]
        public ScavengerElement Scavenger
        {
            get { return (ScavengerElement) this[ScavengerElement]; }
        }


        [ConfigurationProperty("storage", IsRequired = false)]
        public StorageElement StorageProvider
        {
            get { return (StorageElement) this[StorageElement]; }
        }
    }
}