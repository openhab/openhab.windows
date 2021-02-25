using OpenHAB.Core.Model.Connection;

namespace OpenHAB.Core.Contracts
{
    /// <summary>Interface for connection profiles.</summary>
    public interface IConnectionProfile
    {
        /// <summary>Gets a value indicating whether [host URL] value can be modified.</summary>
        /// <value>
        ///   <c>true</c> if [host URL configuration] can be modified; otherwise, <c>false</c>.</value>
        bool AllowHostUrlConfiguration
        {
            get;
        }

        /// <summary>Gets a value indicating whether [allow ignore SSL certificate] issues option is available.</summary>
        /// <value>
        ///   <c>true</c> if [allow ignore SSL certificate] is available; otherwise, <c>false</c>.</value>
        bool AllowIgnoreSSLCertificate
        {
            get;
        }

        /// <summary>Gets a value indicating whether [allow ignore SSL hostname] issue option is available.</summary>
        /// <value>
        /// <c>true.</c> if [allow ignore SSL hostname] can be enabled; otherwise, <c>false</c>.</value>
        bool AllowIgnoreSSLHostname
        {
            get;
        }

        /// <summary>Gets the connection profile îd.</summary>
        /// <value>The îd.</value>
        int Id
        {
            get;
        }

        /// <summary>Gets the connection profile name.</summary>
        /// <value>The profile name.</value>
        string Name
        {
            get;
        }

        /// <summary>Gets the profile type.</summary>
        /// <value>The type.</value>
        OpenHABHttpClientType Type
        {
            get;
        }

        /// <summary>Creates the connection based on the profile.</summary>
        /// <returns>Preconfigured connection.</returns>
        OpenHABConnection CreateConnection();
    }
}