﻿using System.Collections.Generic;
using OpenHAB.Core.Contracts;
using OpenHAB.Core.Model.Connection;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Class that holds all the OpenHAB Windows app settings.
    /// </summary>
    public class Settings
    {
        private static List<IConnectionProfile> _connectionProfiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            IsRunningInDemoMode = false;
            ShowDefaultSitemap = false;
        }

        /// <summary>
        /// Gets or sets the if the default sitemap should be visible.
        /// </summary>
        /// <value>The hide default sitemap.</value>
        public bool ShowDefaultSitemap
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is currently running in demo mode.
        /// </summary>
        public bool? IsRunningInDemoMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the configuration to the OpenHAB remote instance.
        /// </summary>
        public OpenHABConnection LocalConnection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the configuration to the OpenHAB remote instance.
        /// </summary>
        public OpenHABConnection RemoteConnection
        {
            get;
            set;
        }

        /// <summary>Gets the list of available connection profiles.</summary>
        /// <value>The connection profiles.</value>
        public static List<IConnectionProfile> ConnectionProfiles
        {
            get
            {
                if (_connectionProfiles == null)
                {
                    _connectionProfiles = new List<IConnectionProfile>();
                    _connectionProfiles.Add(new LocalConnectionProfile());
                    _connectionProfiles.Add(new DefaultConnectionProfile());
                    _connectionProfiles.Add(new RemoteConnectionProfile());
                    _connectionProfiles.Add(new CloudConnectionProfile());
                }

                return _connectionProfiles;
            }
        }

        /// <summary>
        /// Gets or sets the application language.
        /// </summary>
        /// <value>The application language.</value>
        public string AppLanguage
        {
            get;
            set;
        }
    }
}
