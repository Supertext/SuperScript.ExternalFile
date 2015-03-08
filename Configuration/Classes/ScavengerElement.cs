using System;
using System.Configuration;


namespace SuperScript.ExternalFile.Configuration
{
    /// <summary>
    /// This class represents the superScript.ExternalFile/scavenger configuration section from the web.config file.
    /// </summary>
    public class ScavengerElement : ConfigurationElement
    {
        [ConfigurationProperty("scavengePeriod", IsRequired = false)]
        public TimeSpan? ScavengePeriod
        {
            get
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(this["scavengePeriod"].ToString()))
                    {
                        return TimeSpan.Parse(this["scavengePeriod"].ToString());
                    }
                }
                catch {}

                return null;
            }
        }


        [ConfigurationProperty("scavengeItemsOlderThan", IsRequired = false)]
        public TimeSpan? ScavengeItemsOlderThan
        {
            get
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(this["scavengeItemsOlderThan"].ToString()))
                    {
                        return TimeSpan.Parse(this["scavengeItemsOlderThan"].ToString());
                    }
                }
                catch { }

                return null;
            }
        }
    }
}