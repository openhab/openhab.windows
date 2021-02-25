using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts;

namespace OpenHAB.Core.Model.Connection
{
    /// <summary>Connection profile for for local custom connection to OpenHab server.</summary>
    /// <seealso cref="OpenHAB.Core.Contracts.IConnectionProfile" />
    public class DemoConnectionProfile : IConnectionProfile
    {
        /// <inheritdoc/>
        public bool AllowHostUrlConfiguration
        {
            get => false;
        }

        /// <inheritdoc/>
        public bool AllowIgnoreSSLCertificate
        {
            get => false;
        }

        /// <inheritdoc/>
        public bool AllowIgnoreSSLHostname
        {
            get => false;
        }

        /// <inheritdoc/>
        public int Id
        {
            get => 4;
        }

        /// <inheritdoc/>
        public string Name
        {
            get => AppResources.Values.GetString("DemoConnection");
        }

        /// <inheritdoc/>
        public OpenHABHttpClientType Type
        {
            get => OpenHABHttpClientType.Remote;
        }

        /// <inheritdoc/>
        public OpenHABConnection CreateConnection()
        {
            return new OpenHABConnection()
            {
                Type = OpenHABHttpClientType.Remote,
                Url = Constants.API.DemoModeUrl,
                Username = Constants.API.DemoModeUser,
                Password = Constants.API.DemoModeUserPwd
            };
        }
    }
}
