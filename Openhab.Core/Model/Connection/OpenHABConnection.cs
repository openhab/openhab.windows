using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts;

namespace OpenHAB.Core.Model.Connection
{
    /// <summary>
    /// Connection configuration for OpenHAB service or cloud instance.
    /// </summary>
    public class OpenHABConnection
    {
        /// <summary>Gets or sets the connection profile.</summary>
        /// <value>The profile.</value>
        public IConnectionProfile Profile
        {
            get;
            set;
        }

        /// <summary>Gets or sets the type of the connection.</summary>
        /// <value>The type of the connection.</value>
        public OpenHABHttpClientType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL to the OpenHAB server.
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the username for the OpenHAB server connection.
        /// </summary>
        public string Username
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password for the OpenHAB connection.
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the application will ignore the SSL certificate.
        /// </summary>
        public bool? WillIgnoreSSLCertificate
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the application will ignore the SSL hostname.
        /// </summary>
        public bool? WillIgnoreSSLHostname
        {
            get;
            set;
        }
    }
}
