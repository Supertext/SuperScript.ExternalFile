using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using SuperScript.ExternalFile.ExtensionMethods;
using SuperScript.ExternalFile.Storage;


namespace SuperScript.ExternalFile.Configuration
{
    /// <summary>
    /// This class represents any default configured declarations from the web.config.
    /// </summary>
    public class Settings
    {
		// these are the locations in which we'll look for the handler mappings
	    private readonly string[] _handlerConfigNodeNames =
		    {
			    "system.web",
			    "system.webServer"
		    };
        private TimeSpan _scavengeItemsOlderThan = new TimeSpan(1, 0, 0);
        private TimeSpan _scavengePeriod = new TimeSpan(1, 0, 0);


        #region Properties

	    public HandlerMappings HandlerMappings { get; private set; }


        /// <summary>
        /// <para>Gets or sets how old an <see cref="IStorable"/> should be in order to be scavenged.</para>
        /// <para>Default value is 1 hour.</para>
        /// </summary>
        public TimeSpan ScavengeItemsOlderThan
        {
            get { return _scavengeItemsOlderThan; }
            set { _scavengeItemsOlderThan = value; }
        }


        /// <summary>
        /// <para>Gets or sets how often the ExternalFile scavenger should clear out old instances of <see cref="IStorable"/>.</para>
        /// <para>Default value is 1 hour.</para>
        /// </summary>
        public TimeSpan ScavengePeriod
        {
            get { return _scavengePeriod; }
            set { _scavengePeriod = value; }
        }


        public IStore StoreProvider { get; set; }

        #endregion


        #region Singleton stuff

        private static readonly Settings ThisInstance = new Settings();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static Settings()
        {
        }

        #endregion


	    /// <summary>
	    /// This constructor contains the logic for parsing the configured values out of the web.config.
	    /// </summary>
	    private Settings()
	    {
		    var config = ConfigurationManager.GetSection("superScript.ExternalFile") as ExternalFileConfig;
		    if (config == null)
		    {
			    return;
		    }

	        if (config.Scavenger != null)
	        {
	            if (config.Scavenger.ScavengeItemsOlderThan.HasValue)
	            {
	                ScavengeItemsOlderThan = config.Scavenger.ScavengeItemsOlderThan.Value;
	            }

	            if (config.Scavenger.ScavengePeriod.HasValue)
	            {
	                ScavengePeriod = config.Scavenger.ScavengePeriod.Value;
	            }
	        }

	        StoreProvider = config.StorageProvider.ToStorageProvider();
		    StoreProvider.Init();

		    if (config.StorageProvider.EmptyOnStartup)
		    {
			    StoreProvider.Empty();
		    }

		    foreach (var handlerConfigNodeName in _handlerConfigNodeNames)
		    {
			    HandlerMappings = GetHandlerMappings(handlerConfigNodeName);
			    if (HandlerMappings != null)
			    {
				    break;
			    }
		    }

		    if (HandlerMappings == null)
		    {
			    throw new MissingConfigurationObjectException("Unable to obtain handler configuration from the config file.");
		    }
	    }


	    /// <summary>
        /// Check the web.config file for configured default declarations.
        /// </summary>
        public static Settings Instance
        {
            get { return ThisInstance; }
        }


	    private HandlerMappings GetHandlerMappings(string startingNodeName)
	    {
		    var mappings = new HandlerMappings();

		    var configHandler = ConfigurationManager.GetSection(startingNodeName) as IgnoreSection;
		    if (configHandler == null)
		    {
			    return null;
		    }

		    var fieldRawXml = typeof (IgnoreSection).GetField("_rawXml", BindingFlags.NonPublic | BindingFlags.Instance);
		    if (fieldRawXml == null)
		    {
			    return null;
		    }

		    var xmlConfig = XElement.Parse(fieldRawXml.GetValue(configHandler).ToString());
		    var xmlHandlers = xmlConfig.Descendants().FirstOrDefault(n => n.Name == "handlers");
		    if (xmlHandlers == null)
		    {
			    return null;
		    }
		    var xmlHandlerNodes = xmlHandlers.Descendants().Where(n => n.Name == "add").ToArray();
		    if (!xmlHandlerNodes.Any())
		    {
			    return null;
		    }

		    mappings.Delete = GetHandlerMapping(xmlHandlerNodes, "DeleteHandler");
		    mappings.Empty = GetHandlerMapping(xmlHandlerNodes, "EmptyHandler");
		    mappings.Get = GetHandlerMapping(xmlHandlerNodes, "GetHandler");
		    mappings.Init = GetHandlerMapping(xmlHandlerNodes, "InitHandler");
		    mappings.List = GetHandlerMapping(xmlHandlerNodes, "ListHandler");
		    mappings.ReInit = GetHandlerMapping(xmlHandlerNodes, "ReInitHandler");
		    mappings.Remove = GetHandlerMapping(xmlHandlerNodes, "RemoveHandler");

		    return mappings;
	    }


	    private string GetHandlerMapping(IEnumerable<XElement> xmlHandlerNodes, string handlerName)
	    {
		    var handler = xmlHandlerNodes.FirstOrDefault(n => n.Attribute("name").Value == handlerName);
		    return handler == null
			           ? String.Empty
			           : handler.Attribute("path").Value;
	    }
    }
}