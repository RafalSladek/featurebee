﻿using System.Configuration;

namespace FeatureBee.Configuration
{
    public class FeatureBeeServerConfig: ConfigurationElement
    {
        [ConfigurationProperty("url", DefaultValue = "", IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
            set { this["url"] = value; }
        }

        [ConfigurationProperty("updateMode", DefaultValue = UpdateMode.Pull, IsRequired = false)]
        public UpdateMode UpdateMode
        {
            get { return (UpdateMode)this["updateMode"]; }
            set { this["updateMode"] = value; }
        }
    }
}