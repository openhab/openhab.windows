using System;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts;

namespace OpenHAB.Core.Model.Connection
{
    /// <summary>Connection profile for local custom connection to OpenHab server.</summary>
    /// <seealso cref="OpenHAB.Core.Contracts.IConnectionProfile" />
    public class LocalConnectionProfile : IConnectionProfile
    {
        /// <inheritdoc/>
        public bool AllowHostUrlConfiguration
        {
            get => true;
        }

        /// <inheritdoc/>
        public bool AllowIgnoreSSLCertificate
        {
            get => true;
        }

        /// <inheritdoc/>
        public bool AllowIgnoreSSLHostname
        {
            get => true;
        }

        /// <summary>Gets the connection profile îd.</summary>
        /// <value>The îd.</value>
        public int Id
        {
            get => 2;
        }

        /// <inheritdoc/>
        public string Name
        {
            get => AppResources.Values.GetString("LocalCustomConnection");
        }

        /// <inheritdoc/>
        public OpenHABHttpClientType Type
        {
            get => OpenHABHttpClientType.Local;
        }

        /// <inheritdoc/>
        public OpenHABConnection CreateConnection()
        {
            return new OpenHABConnection()
            {
                Type = OpenHABHttpClientType.Local,
            };
        }
    }
}
