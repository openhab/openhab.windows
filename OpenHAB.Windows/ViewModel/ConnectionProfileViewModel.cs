using OpenHAB.Core.Model.Connection;
using OpenHAB.Windows.ViewModel;

namespace OpenHAB.Core.Contracts
{
    /// <summary>
    /// ViewModel for connection profile.
    /// </summary>
    public class ConnectionProfileViewModel : ViewModelBase<IConnectionProfile>
    {
        /// <summary>Initializes a new instance of the <see cref="ConnectionProfileViewModel"/> class.</summary>
        /// <param name="profile">The profile.</param>
        public ConnectionProfileViewModel(IConnectionProfile profile)
            : base(profile)
        {
        }

        /// <summary>Gets a value indicating whether [host URL] value can be modified.</summary>
        /// <value>
        ///   <c>true</c> if [host URL configuration] can be modified; otherwise, <c>false</c>.</value>
        public bool AllowHostUrlConfiguration => Model.AllowHostUrlConfiguration;

        /// <summary>Gets a value indicating whether [allow ignore SSL certificate] issues option is available.</summary>
        /// <value>
        ///   <c>true</c> if [allow ignore SSL certificate] is available; otherwise, <c>false</c>.
        /// </value>
        public bool AllowIgnoreSSLCertificate => Model.AllowIgnoreSSLCertificate;

        /// <summary>Gets a value indicating whether [allow ignore SSL hostname] issue option is available.</summary>
        /// <value>
        /// <c>true.</c> if [allow ignore SSL hostname] can be enabled; otherwise, <c>false.</c>.
        /// </value>
        public bool AllowIgnoreSSLHostname => Model.AllowIgnoreSSLHostname;

        /// <summary>Gets the connection profile id.</summary>
        /// <value>The id.</value>
        public int Id => Model.Id;

        /// <summary>Gets the connection profile name.</summary>
        /// <value>The profile name.</value>
        public string Name => Model.Name;

        /// <summary>Gets the profile type.</summary>
        /// <value>The type.</value>
        public OpenHABHttpClientType Type => Model.Type;
    }
}